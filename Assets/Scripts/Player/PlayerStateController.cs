using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerStateController : MonoBehaviour, IHittable
{
    public enum State { Idle, Walking, Running, Falling, Blocking, Attacking, LandRolling, TakeHit, BlockHit, Die, Size };
    public State CurState { get { return curState; } set { curState = value; OnStateChanged?.Invoke(CurState); } }
    public Vector3 MoveDir { get { return moveDir; } }

    public UnityAction<State> OnStateChanged;

    [SerializeField] private State curState;
    private Vector3 moveDir;
    private Animator animator;
    private PlayerStatusController statusController;
    private PlayerMover mover;
    private PlayerAttacker attacker;
    private PlayerHitter hitter;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        statusController = GetComponent<PlayerStatusController>();
        mover = GetComponent<PlayerMover>();
        attacker = GetComponent<PlayerAttacker>();
        hitter = GetComponent<PlayerHitter>();
        
        CurState = State.Idle;
    }
    private void Start()
    {
        mover.moveRoutine = mover.StartCoroutine(mover.MoveRoutine());
        mover.fallRoutine = mover.StartCoroutine(mover.FallRoutine());
        
    }
    private void OnEnable()
    {
        mover.lookRoutine = mover.StartCoroutine(mover.LookRoutine());
        OnStateChanged += ChangeState;
    }

    private void OnDisable()
    {
        StopCoroutine(mover.lookRoutine);
        OnStateChanged -= ChangeState; 
    }

    private void Update()
    {
        MoveDirCheck();
        GroundCheck();
        MovingCheck();
    }

    private void ChangeState(State state)
    {
        animator.SetInteger("CurState", (int)state);

        if (state == State.Idle) { }
        else if (state == State.Walking || state == State.Running)
        {
            if (mover.moveRoutine != null)
                mover.StopCoroutine(mover.moveRoutine);
            mover.moveRoutine = mover.StartCoroutine(mover.MoveRoutine());
        }
        else if (state == State.Blocking || state == State.Attacking)
        {
            if (mover.moveRoutine != null)
                mover.StopCoroutine(mover.moveRoutine);
            mover.moveRoutine = mover.StartCoroutine(mover.MoveRoutine());
            animator.SetLayerWeight(1, 1);
            attacker.attackRoutine = attacker.StartCoroutine(attacker.AttackRoutine());
        }
        else if (state == State.LandRolling)
        {
            if(mover.lookRoutine != null)
                mover.StopCoroutine(mover.lookRoutine);
            if(mover.moveRoutine != null)
                mover.StopCoroutine(mover.moveRoutine);
            if (attacker.attackRoutine != null)
                attacker.StopCoroutine(attacker.attackRoutine);
            mover.StartCoroutine(mover.LandRollRoutine());           
        }
        else if (state == State.TakeHit || state == State.BlockHit)
        {
            if (mover.moveRoutine != null)
                mover.StopCoroutine(mover.moveRoutine);
            if (attacker.attackRoutine != null)
                attacker.StopCoroutine(attacker.attackRoutine);
            if (hitter.hitRoutine != null)
                hitter.StopCoroutine(hitter.hitRoutine);
            hitter.hitRoutine = hitter.StartCoroutine(hitter.HitRoutine(hitDamage));
        }
    }

    // MoveSpeed�� 0 �ʰ���� Walking
    // 0 ���϶�� Idle
    private void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        moveDir = new Vector3(input.x, 0, input.y);
        
        if(curState <= State.Walking)
        {
            CurState = (State)this.IsMoving();
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
        else
        {
            CurState = (State)this.IsMoving();
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

    private void MoveDirCheck()
    {
        animator.SetFloat("XInput", moveDir.x, 0.1f, Time.deltaTime);
        animator.SetFloat("YInput", moveDir.z, 0.1f, Time.deltaTime);
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
                CurState = (State)this.IsMoving();
            }
        }
    }

    private void MovingCheck()
    {
        if(this.IsMoving() > 0)
        {
            animator.SetBool("IsMoving", true);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }

    // Idle �̳� Walking ������ ���� �̹� ���� ���� �� Attack
    private void OnAttack(InputValue inputValue)
    {
        // Idle �̳� Walking �Ǵ� Attack �� �� IsAttacking�� true
        if (CurState <= State.Walking)
        {
            animator.SetBool("ContinuousAttack", true);
            CurState = State.Attacking;
        }
        else if (CurState == State.Attacking)
        {
            animator.SetBool("ContinuousAttack", true);
        }
    }

    // Idle �̳� Walking ������ �� Block
    private void OnBlock(InputValue inputValue)
    {
        // Idle �̳� Walking�� �ƴ� ��� return
        if (CurState > State.Walking && CurState != State.Blocking)
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
            CurState = (State)this.IsMoving();
        }
    }

    private float hitDamage;
    public void TakeHit(float damage, GameObject attacker)
    {
        hitDamage = damage;
        if (CurState == State.Blocking)
        {
            CurState = State.BlockHit;
        }
        else
        {
            CurState = State.TakeHit;
        }               
    }

    public void Die()
    {
        // TODO die
    }
}
