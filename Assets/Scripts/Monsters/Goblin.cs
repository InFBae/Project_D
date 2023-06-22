using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using static UnityEngine.UI.GridLayoutGroup;

public class Goblin : Monster
{
    [SerializeField] Collider attackCollider;
    public enum State { Idle, Trace, Attack, Return, TakeHit, Die, Size }
    StateMachine<State, Goblin> stateMachine;

    private bool[] skillAvailability;
    private float[] skillCoolTime;

    [SerializeField] private Transform spawnPoint;
    private bool isAttacking = false;

    protected override void Awake()
    {
        base.Awake();
        InitData();

        attackCollider.enabled = false;

        stateMachine = new StateMachine<State, Goblin>(this);
        stateMachine.AddState(State.Idle, new IdleState(this, stateMachine));
        stateMachine.AddState(State.Trace, new TraceState(this, stateMachine));
        stateMachine.AddState(State.Attack, new AttackState(this, stateMachine));
        stateMachine.AddState(State.Return, new ReturnState(this, stateMachine));
        stateMachine.AddState(State.TakeHit, new TakeHitState(this, stateMachine));
        stateMachine.AddState(State.Die, new DieState(this, stateMachine));
    }

    private void Start()
    {
        stateMachine.SetUp(State.Idle);
    }
    private void Update()
    {
        stateMachine.Update();
    }
    public override void TakeHit(float damage, GameObject attacker)
    {
        StopAllCoroutines();
        CurHP -= damage;
        attackCollider.enabled = false;
        isAttacking = false;
        if (CurHP <= 0)
        {
            stateMachine.ChangeState(State.Die);
        }
        else
        {
            target = attacker;
            stateMachine.ChangeState(State.TakeHit);
        }
    }

    public override void Die()
    {
        StopAllCoroutines();
        animator.SetTrigger("die");
        rb.isKinematic = true;
        coll.enabled = false;
        GameManager.Resource.Destroy(gameObject, 5f);
    }

    private void InitData()
    {
        monsterData = GameManager.Resource.Load<MonsterData>("Data/Monsters/GoblinData");
        CurHP = monsterData.maxHP;

        range = monsterData.detectRange;
        angle = Mathf.Cos(120f * 0.5f * Mathf.Deg2Rad);
        targetMask = (1 << LayerMask.NameToLayer("Player"));
        obstacleMask = (1 << LayerMask.NameToLayer("Environment"));

        InitSkillData();
    }

    private void InitSkillData()
    {
        skillAvailability = new bool[4];
        for (int i = 0; i < skillAvailability.Length; i++)
        {
            skillAvailability[i] = true;
        }
        skillCoolTime = new float[4];
        skillCoolTime[0] = 3;
        skillCoolTime[1] = 8;
        skillCoolTime[2] = 3;
        skillCoolTime[3] = 5;

    }

    Coroutine attackRoutine;
    IEnumerator AttackRoutine()
    {
        
        isAttacking = true;
        hitTable.Clear();
        attackCollider.enabled = true;

        float skill1Range = 3f;

        if (lookRoutine != null)
            StopCoroutine(lookRoutine);

        if (moveRoutine != null)
            StopCoroutine(moveRoutine);
        animator.SetBool("move", false);

        if (skillAvailability[0] && Vector3.Distance(target.transform.position, transform.position) >= skill1Range)
        {
            animator.SetTrigger("attack1");
            yield return new WaitForSeconds(1.6f);
            skillAvailability[0] = false;
            StartCoroutine(TimerRoutine(skillCoolTime[0], 0));
        }
        else if (skillAvailability[1])
        {
            animator.SetTrigger("attack2");
            yield return new WaitForSeconds(2.66f);
            skillAvailability[1] = false;
            StartCoroutine(TimerRoutine(skillCoolTime[1], 1));
        }
        else if (skillAvailability[2])
        {
            animator.SetTrigger("attack3");
            yield return new WaitForSeconds(2.2f);
            skillAvailability[2] = false;
            StartCoroutine(TimerRoutine(skillCoolTime[2], 2));
        }
        else if (skillAvailability[3])
        {
            animator.SetTrigger("attack4");
            yield return new WaitForSeconds(2.8f);
            skillAvailability[3] = false;
            StartCoroutine(TimerRoutine(skillCoolTime[3], 3));
        }

        attackCollider.enabled = false;

        if (Vector3.Distance(target.transform.position, transform.position) > monsterData.attackRange)
            stateMachine.ChangeState(State.Trace);

        if (lookRoutine != null)
            StopCoroutine(lookRoutine);
        lookRoutine = StartCoroutine(LookRoutine());
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(MoveRoutine());

        yield return new WaitForSeconds(monsterData.attackCooltime);

        isAttacking = false;
    }

    IEnumerator TimerRoutine(float coolTime, int index)
    {
        float currentTime = 0;

        while (currentTime < coolTime)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }
        skillAvailability[index] = true;
    }

    Coroutine lookRoutine;
    IEnumerator LookRoutine()
    {
        while (target != null)
        {
            Quaternion lookRotation = Quaternion.LookRotation(target.transform.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, 10 * Time.deltaTime);
            yield return null;
        }
    }

    Coroutine moveRoutine;
    IEnumerator MoveRoutine()
    {
        animator.SetBool("move", true);
        float currentTime = 0f;
        while (currentTime < monsterData.attackCooltime * 0.9)
        {
            currentTime = Time.deltaTime;
            rb.MovePosition(transform.position + transform.forward * monsterData.speed * Time.deltaTime);
            yield return null;
        }
        animator.SetBool("move", false);
    }

    private void OnTriggerEnter(Collider other)
    {
        IHittable hittable = other.GetComponent<IHittable>();
        if (hittable != null)
        {
            if (hitTable.TryAdd(hittable, monsterData.damage))
                hittable.TakeHit(monsterData.damage, gameObject);
        }
    }

    Coroutine takeHitRoutine;
    IEnumerator TakeHitRoutine()
    {
        float currentTime = 0;
        animator.SetTrigger("weakHit");
        while (currentTime < 0.8f)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }
        if (lookRoutine != null)
            StopCoroutine(lookRoutine);
        lookRoutine = StartCoroutine(LookRoutine());
        yield return new WaitForSeconds(monsterData.attackCooltime);
        stateMachine.ChangeState(State.Trace);
    }

    Coroutine returnRoutine;
    IEnumerator ReturnRoutine()
    {
        while (target == null)
        {
            transform.LookAt(spawnPoint);
            rb.MovePosition(transform.position + transform.forward * monsterData.speed * Time.deltaTime);
            FindTarget();
            if (Vector3.Distance(transform.position, spawnPoint.position) < 0.1f)
            {
                animator.SetBool("move", false);
                
                transform.rotation = spawnPoint.rotation;
                stateMachine.ChangeState(State.Idle);
                yield break;
            }
            yield return null;
        }
        stateMachine.ChangeState(State.Trace);
    }

    float range;
    float angle;
    LayerMask targetMask;
    LayerMask obstacleMask;
    public void FindTarget()
    {
        // 1. 범위
        Collider[] targets = Physics.OverlapSphere(transform.position + (Vector3.up * 1), range, targetMask);
        for (int i = 0; i < targets.Length; i++)
        {
            Vector3 dirToTarget = (targets[i].transform.position - transform.position).normalized;

            // 2. 각도
            if (Vector3.Dot(transform.forward, dirToTarget) < angle)
                continue;

            // 3. 중간 장애물
            float distToTarget = Vector3.Distance(transform.position + (Vector3.up * 1), targets[i].transform.position);
            if (Physics.Raycast(transform.position + (Vector3.up * 1), dirToTarget, distToTarget, obstacleMask))
                continue;

            target = targets[i].gameObject;
            return;
        }
        target = null;
    }

    #region GoblinState
    private abstract class GoblinState : StateBase<State, Goblin>
    {
        protected GameObject gameObject => owner.gameObject;
        protected Transform transform => owner.transform;
        protected Renderer rend => owner.rend;
        protected Animator anim => owner.animator;
        protected Collider coll => owner.coll;
        protected GoblinState(Goblin owner, StateMachine<State, Goblin> stateMachine) : base(owner, stateMachine)
        {
        }
    }

    private class IdleState : GoblinState
    { 
        public IdleState(Goblin owner, StateMachine<State, Goblin> stateMachine) : base(owner, stateMachine)
        {
        }

        public override void Enter()
        {
            Debug.Log("IdleEnter");
            owner.target = null;
        }

        public override void Exit()
        {
        }

        public override void Setup()
        {
        }

        public override void Transition()
        {
            if (owner.target != null)
            {
                stateMachine.ChangeState(State.Trace);
            }
        }

        public override void Update()
        {
            owner.FindTarget();
        }

        
    }
    private class TraceState : GoblinState
    {
        public TraceState(Goblin owner, StateMachine<State, Goblin> stateMachine) : base(owner, stateMachine)
        {
        }

        public override void Enter()
        {
            Debug.Log("TraceEnter");
            if (owner.lookRoutine != null)
                owner.StopCoroutine(owner.lookRoutine);
            owner.lookRoutine = owner.StartCoroutine(owner.LookRoutine());
            owner.animator.SetBool("move", true);
        }

        public override void Exit()
        {
            owner.animator.SetBool("move", false);
        }

        public override void Setup()
        {
        }

        public override void Transition()
        {
            if (Vector3.Distance(owner.target.transform.position, transform.position) < owner.monsterData.attackRange)
            {                
                stateMachine.ChangeState(State.Attack);
            }
            if (Vector3.Distance(owner.target.transform.position, transform.position) > owner.monsterData.detectRange)
            {
                owner.target = null;
                stateMachine.ChangeState(State.Return);
            }            
        }

        public override void Update()
        {
            owner.rb.MovePosition(owner.transform.position + owner.transform.forward * owner.monsterData.speed * Time.deltaTime);
        }

    }

    private class AttackState : GoblinState
    {
        public AttackState(Goblin owner, StateMachine<State, Goblin> stateMachine) : base(owner, stateMachine)
        {
        }

        public override void Enter()
        {
            Debug.Log("attackEnter"); 
        }

        public override void Exit()
        {
            if(owner.attackRoutine != null) 
                owner.StopCoroutine(owner.attackRoutine);
            owner.isAttacking = false;
        }

        public override void Setup()
        {
        }

        public override void Transition()
        {
            if(!owner.isAttacking && Vector3.Distance(owner.target.transform.position, transform.position) > owner.monsterData.attackRange)
            {
                stateMachine.ChangeState(State.Trace);
            }
        }

        public override void Update()
        {
            if (owner.isAttacking)
            {
                return;
            }
            if (owner.attackRoutine != null)
                owner.StopCoroutine(owner.attackRoutine);
            owner.attackRoutine = owner.StartCoroutine(owner.AttackRoutine());
        }
    }

    private class ReturnState : GoblinState
    {
        public ReturnState(Goblin owner, StateMachine<State, Goblin> stateMachine) : base(owner, stateMachine)
        {
        }

        public override void Enter()
        {
            Debug.Log("returnEnter");
            owner.animator.SetBool("move", true);
            if (owner.returnRoutine != null)
                owner.StopCoroutine(owner.returnRoutine);
            owner.returnRoutine = owner.StartCoroutine(owner.ReturnRoutine());
        }

        public override void Exit()
        {
            owner.animator.SetBool("move", false);
            if (owner.returnRoutine != null)
                owner.StopCoroutine(owner.returnRoutine);
        }

        public override void Setup()
        {
        }

        public override void Transition()
        {
        }

        public override void Update()
        {
        }
    }
    private class TakeHitState : GoblinState
    {
        public TakeHitState(Goblin owner, StateMachine<State, Goblin> stateMachine) : base(owner, stateMachine)
        {
        }

        public override void Enter()
        {
            Debug.Log("TakeHitEnter");
            if (owner.takeHitRoutine != null)
                owner.StopCoroutine(owner.takeHitRoutine);
            owner.StartCoroutine(owner.TakeHitRoutine());
        }

        public override void Exit()
        {
        }

        public override void Setup()
        {
        }

        public override void Transition()
        {
        }

        public override void Update()
        {
        }
    }
    private class DieState : GoblinState
    {
        public DieState(Goblin owner, StateMachine<State, Goblin> stateMachine) : base(owner, stateMachine)
        {
        }

        public override void Enter()
        {
            owner.Die();
        }

        public override void Exit() { }

        public override void Setup() { }

        public override void Transition() { }

        public override void Update() { }
    }
    #endregion
}