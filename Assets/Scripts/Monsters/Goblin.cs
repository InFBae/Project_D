using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Monster
{
    [SerializeField] Collider attackCollider;
    public enum State { Idle, Trace, Attack, Return, TakeHit, Die, Size }
    StateMachine<State, Goblin> stateMachine;

    [SerializeField] private GameObject target;
    private bool isAttacking = false;

    protected override void Awake()
    {
        base.Awake();
        InitData();

        attackCollider.enabled = false;

        stateMachine = new StateMachine<State, Goblin>(this);
        stateMachine.AddState(State.Idle, new IdleState(this, stateMachine));
        stateMachine.AddState(State.Trace, new TraceState(this, stateMachine));

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
        CurHP -= damage;
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
        GameManager.Resource.Destroy(gameObject, 5f);
    }

    private void InitData()
    {
        monsterData = GameManager.Resource.Load<MonsterData>("Data/Monsters/GoblinData");
        CurHP = monsterData.maxHP;
    }

    Coroutine attackRoutine;
    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        hitTable.Clear();

        if (lookRoutine != null)
        {
            StopCoroutine(lookRoutine);
        }

        if (Vector3.Distance(target.transform.position, transform.position) < monsterData.attackRange)
        {
            animator.SetTrigger("attack2");
        }
        else
        {
            animator.SetTrigger("attack1");
        }
        yield return new WaitForSeconds(monsterData.attackCooltime);

        lookRoutine = StartCoroutine(LookRoutine());
        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }

    Coroutine lookRoutine;
    IEnumerator LookRoutine()
    {
        while (target != null)
        {
            Quaternion lookRotation = Quaternion.LookRotation(target.transform.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, 3 * Time.deltaTime);
            yield return null;
        }
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
    IEnumerator TakeHitRoutine()
    {
        float currentTime = 0;
        animator.SetTrigger("gotHit");
        while (currentTime < 0.6f)
        {
            yield return null;
        }
        stateMachine.ChangeState(State.Trace);
    }

    #region SlimeState
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
        float range;
        float angle;
        LayerMask targetMask;
        LayerMask obstacleMask;

        public IdleState(Goblin owner, StateMachine<State, Goblin> stateMachine) : base(owner, stateMachine)
        {
        }

        public override void Enter()
        {
            owner.target = null;
        }

        public override void Exit()
        {
        }

        public override void Setup()
        {
            range = owner.monsterData.detectRange;
            angle = Mathf.Cos(120f * 0.5f * Mathf.Deg2Rad);
            targetMask = (1 << LayerMask.NameToLayer("Player"));
            obstacleMask = (1 << LayerMask.NameToLayer("Environment"));
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
            FindTarget();
        }

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

                owner.target = targets[i].gameObject;
                return;
            }
            owner.target = null;
        }
    }
    private class TraceState : GoblinState
    {
        public TraceState(Goblin owner, StateMachine<State, Goblin> stateMachine) : base(owner, stateMachine)
        {
        }

        public override void Enter()
        {
        }

        public override void Exit()
        {
        }

        public override void Setup()
        {
        }

        public override void Transition()
        {
            if (Vector3.Distance(owner.target.transform.position, transform.position) > owner.monsterData.detectRange)
            {
                owner.target = null;
                stateMachine.ChangeState(State.Idle);
            }
        }

        public override void Update()
        {
            if (owner.isAttacking)
            {
                return;
            }
            if (owner.lookRoutine != null)
                owner.StopCoroutine(owner.lookRoutine);
            owner.lookRoutine = owner.StartCoroutine(owner.LookRoutine());

            owner.StartCoroutine(owner.AttackRoutine());
        }

    }

    private class TakeHitState : GoblinState
    {
        public TakeHitState(Goblin owner, StateMachine<State, Goblin> stateMachine) : base(owner, stateMachine)
        {
        }

        public override void Enter()
        {
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