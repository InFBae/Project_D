using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerStateController : MonoBehaviour
{
    public enum State { Idle, Walking, Running, Falling, Blocking, Attacking, LandRolling, Die, Size };
    public State CurState { get { return curState; } set { curState = value; OnStateChanged?.Invoke(CurState); } }
    public Vector3 MoveDir { get { return moveDir; } }

    public UnityAction<State> OnStateChanged;

    [SerializeField] private State curState;
    private Vector3 moveDir;
    private Animator animator;
    private PlayerStatusController statusController;
    private PlayerMover mover;
    private PlayerAttacker attacker;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        statusController = GetComponent<PlayerStatusController>();
        mover = GetComponent<PlayerMover>();
        attacker = GetComponent<PlayerAttacker>();
        
        CurState = State.Idle;
    }
    private void Start()
    {
        mover.fallRoutine = StartCoroutine(mover.FallRoutine());
    }
    private void OnEnable()
    {
        OnStateChanged += ChangeState;
    }

    private void OnDisable()
    {
        StopCoroutine(mover.fallRoutine);
        OnStateChanged -= ChangeState; 
    }

    private void Update()
    {
        animator.SetFloat("XInput", moveDir.x, 0.1f, Time.deltaTime);
        animator.SetFloat("YInput", moveDir.z, 0.1f, Time.deltaTime);
        GroundCheck();
        mover.Look();
    }

    private void ChangeState(State state)
    {
        animator.SetInteger("CurState", (int)state);

        if (state == State.Idle) { }
        else if (state == State.Walking || state == State.Running)
        {
            if (mover.moveRoutine != null)
                StopCoroutine(mover.moveRoutine);
            mover.moveRoutine = StartCoroutine(mover.MoveRoutine());
        }
        else if (state == State.Blocking)
        {
            animator.SetLayerWeight(1, 1);
        }
        else if (state == State.Attacking)
        {
            
        }
        else if (state == State.LandRolling)
        {
            mover.StartCoroutine(mover.LandRollRoutine());
        }
    }

    // MoveSpeed�� 0 �ʰ���� Walking
    // 0 ���϶�� Idle
    private void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        moveDir = new Vector3(input.x, 0, input.y);
        
        if(curState != State.Falling && curState != State.LandRolling)
        {
            if (input.sqrMagnitude > 0)
            {
                CurState = State.Walking;
                animator.SetInteger("CurState", (int)State.Walking);
            }
            else
            {
                curState = State.Idle;
                animator.SetInteger("CurState", (int)State.Idle);
            }
        }      
    }

    // �÷��̾ ������ ������ �����̰� �ְ�
    // �ٸ� �ൿ ���� �ƴ� �� run Ű�� �ԷµǾ��ٸ�
    // Running���� ����

    private void OnRun(InputValue value)
    {
        // Idle �̳� Walking ���°� �ƴ϶�� ���¸� �������� �ʴ´�.
        if (CurState > State.Running )
        {
            return;
        }
        bool running = value.isPressed;

        if (running &&
           animator.GetFloat("YInput") > 0.1 &&
           statusController.GetCurrentSP() > 1 * Time.deltaTime)
        {
            CurState = State.Running;
        }
        else if(MoveDir.sqrMagnitude > 0.1)
        {
            CurState = State.Walking;
        }
        else
        {
            CurState = State.Idle;
        }
    }

    // �����Ⱑ �ԷµǾ��� �� ���� ��� �ְ�
    // ������ ���� �ƴ� ��
    // SP�� ����ϴٸ� ��� ������
    private void OnLandRoll(InputValue value)
    {
        if (CurState != State.Falling && 
            CurState != State.LandRolling &&
            statusController.GetCurrentSP() >= 2)
        {
            CurState = State.LandRolling;
        }
    }

    // ���� ��� �ִ��� üũ
    // ���߿� �ִٸ� Falling
    private void GroundCheck()
    {
        RaycastHit hit;

        if (!Physics.SphereCast(transform.position + Vector3.up * 1f, 0.5f, Vector3.down, out hit, 0.8f))
            CurState = State.Falling;
        else
        {
            if(CurState == State.Falling)
            {
                if (MoveDir.sqrMagnitude > 0.1) { CurState = State.Walking; }
                else { CurState = State.Idle; }
            }
        }
    }

    // Idle �̳� Walking ������ ���� �̹� ���� ���� �� Attack
    private void OnAttack(InputValue inputValue)
    {
        // Idle �̳� Walking �Ǵ� Attack �� �� Attack Ʈ���� �ߵ�
        if (CurState <= State.Walking || CurState == State.Attacking)
        {
            animator.SetTrigger("Attack");
        }
        else
        {
            animator.ResetTrigger("Attack");
        }      
    }

    // Idle �̳� Walking ������ �� Block
    private void OnBlock(InputValue inputValue)
    {
        // Idle �̳� Walking�� �ƴ� ��� return
        if (CurState > State.Walking)
        {
            return;
        }
        // Block �������� �ִ� ���� CurState = Blocking
        bool isBlocking = inputValue.isPressed;
        if(isBlocking)
        {
            CurState = State.Blocking;
        }
        else
        {

        }
    }
}
