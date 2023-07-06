using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerStateController : MonoBehaviour, IHittable
{
    public enum State { Idle, Walking, Running, Falling, Blocking, Attacking, LandRolling, TakeHit, BlockHit, Die, StrongAttack, Size };
    public State CurState { get { return curState; } set { curState = value; OnStateChanged?.Invoke(CurState); } }
    public Vector3 MoveDir { get { return moveDir; } }

    public UnityAction<State> OnStateChanged;
    public static UnityAction OnPlayerDied;
    public IHittable.HitType attackType;

    [SerializeField] private State curState;
    private Vector3 moveDir;
    private Animator animator;
    private PlayerStatusController statusController;
    private PlayerMover mover;
    private PlayerAttacker attacker;
    private PlayerHitter hitter;

    private PlayerInput playerInput;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
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
        mover.StartRoutines();
        OnStateChanged += ChangeState;
        OnStateChanged += SetPlayerState;
        OnPlayerDied += Die;
    }

    private void OnDisable()
    {
        mover.StopRoutines();
        OnStateChanged -= ChangeState;
        OnStateChanged -= SetPlayerState;
        OnPlayerDied -= Die;
    }

    private void Update()
    {
        MoveDirCheck();
        GroundCheck();
        MovingCheck();
        CheckOnUI();
    }

    private void SetPlayerState(State state)
    {
        GameManager.Data.PlayerState = state;
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
            attackType = IHittable.HitType.Weak;
            if (mover.moveRoutine != null)
                mover.StopCoroutine(mover.moveRoutine);
            mover.moveRoutine = mover.StartCoroutine(mover.MoveRoutine());
            animator.SetLayerWeight(1, 1);
            attacker.attackRoutine = attacker.StartCoroutine(attacker.AttackRoutine());
        }
        else if (state == State.StrongAttack)
        {
            attackType = IHittable.HitType.Strong;
            if (mover.moveRoutine != null)
                mover.StopCoroutine(mover.moveRoutine);
            attacker.StartCoroutine(attacker.StrongAttackRoutine());
            
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
            hitter.hitRoutine = hitter.StartCoroutine(hitter.HitRoutine(hitDamage, hitType));
        }
        else if (state == State.Die)
        {
            mover.StopRoutines();
            attacker.StopAllCoroutines();
            hitter.StopAllCoroutines();
            //Destroy(gameObject, 5f);
        }
    }

    // MoveSpeed가 0 초과라면 Walking
    // 0 이하라면 Idle
    private void OnMove(InputValue value)
    {
        if (curState == State.Die) return;
        Vector2 input = value.Get<Vector2>();
        moveDir = new Vector3(input.x, 0, input.y);
        
        if(curState <= State.Walking)
        {
            CurState = (State)this.IsMoving();
        }      
    }

    // 플레이어가 땅에서 앞으로 움직이고 있고
    // 다른 행동 중이 아닐 때 run 키가 입력되었다면
    // Running으로 설정

    private void OnRun(InputValue value)
    {
        if (curState == State.Die) return;
        // Idle 이나 Walking 상태가 아니라면 상태를 변경하지 않는다.
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

    // 구르기가 입력되었을 때 땅에 닿아 있고
    // 구르기 중이 아닐 때
    // SP가 충분하다면 즉시 구르기
    private void OnLandRoll(InputValue value)
    {
        if (curState == State.Die) return;
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

    // 땅에 닿아 있는지 체크
    // 공중에 있다면 Falling
    private void GroundCheck()
    {
        RaycastHit hit;

        if (!Physics.SphereCast(transform.position + Vector3.up * 1f, 0.5f, Vector3.down, out hit, 0.9f))
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

    private void OnAttack(InputValue inputValue)
    {
        if (curState == State.Die) return;
        if (statusController.GetCurrentSP() < 1.5f) return;
        if (animator.GetBool("StrongKey") && statusController.GetCurrentSP() >= 3f)
        {
            if (animator.GetBool("IsAttacking"))
                return;
            CurState = State.StrongAttack;
        }
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

    // Idle 이나 Walking 상태일 때 Block
    private void OnBlock(InputValue inputValue)
    {
        // Idle 이나 Walking이 아닐 경우 return
        if (CurState > State.Walking && CurState != State.Blocking)
        {
            return;
        }
        // Block 눌러지고 있는 동안 CurState = Blocking
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

    private void OnStrongKey(InputValue input)
    {
        if (input.isPressed)
        {
            animator.SetBool("StrongKey", true);
        }
        else
            animator.SetBool("StrongKey", false);
    }

    private float hitDamage;
    private IHittable.HitType hitType;
    public void TakeHit(float damage, GameObject attacker, IHittable.HitType hitType)
    {
        if (CurState == State.LandRolling) return;

        hitDamage = damage;
        this.hitType = hitType;

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
        CurState = State.Die;
        playerInput.enabled = false;
        GameManager.Sound.Play("PlayerDie");
    }

    public void PopUpGameOverUI()
    {
        statusController.DIsableStatusSceneUI();
        Cursor.lockState = CursorLockMode.None;
        GameManager.Sound.Play("GameOver");
        GameManager.UI.ShowPopUpUI<GameOverUI>("UI/GameOverUI");
    }

    public void CheckOnUI()
    {
        if (CurState == State.Die) { return; }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            playerInput.enabled = false;
        }
        else
        {
            playerInput.enabled = true;
        }
    }
}
