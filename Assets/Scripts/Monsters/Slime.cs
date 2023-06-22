using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.GridLayoutGroup;

public class Slime : Monster
{
    [SerializeField] Collider attackCollider;

    public enum State { Idle, Trace, Return, TakeHit, Die, Size }
    StateMachine<State, Slime> stateMachine;
    
    [SerializeField] private bool isAttacking = false;

    protected override void Awake()
    {
        base.Awake();
        InitData();
        
        attackCollider.gameObject.SetActive(false);

        stateMachine = new StateMachine<State, Slime>(this);
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
        
        StopAllCoroutines();
        isAttacking = false;
        CurHP -= damage;
        if(CurHP <= 0)
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
        monsterData = GameManager.Resource.Load<MonsterData>("Data/Monsters/SlimeData");
        CurHP = monsterData.maxHP;        
    }

    Coroutine attackRoutine;
    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        hitTable.Clear();

        if (lookRoutine != null)
            StopCoroutine(lookRoutine);

        if (Vector3.Distance(target.transform.position, transform.position) > monsterData.attackRange)
        {
            animator.SetTrigger("attack2");
            //rb.AddForce(Vector3.up * 10f + Vector3.forward * 8f, ForceMode.Impulse);
        }
        else
        {
            animator.SetTrigger("attack1");
            yield return new WaitForSeconds(0.5f);
            attackCollider.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            attackCollider.gameObject.SetActive(false);
        }
        if (lookRoutine != null)
            StopCoroutine(lookRoutine);
        lookRoutine = StartCoroutine(LookRoutine());
        yield return new WaitForSeconds(monsterData.attackCooltime);
        
        
        isAttacking = false;        
    }

    Coroutine lookRoutine;
    IEnumerator LookRoutine()
    {
        while(target != null)
        {
            Quaternion lookRotation = Quaternion.LookRotation(target.transform.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, 5 * Time.deltaTime);
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

    Coroutine takeHitRoutine;
    IEnumerator TakeHitRoutine()
    {
        float currentTime = 0;
        animator.SetTrigger("gotHit");
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

    #region SlimeState
    private abstract class SlimeState : StateBase<State, Slime>
    {
        protected GameObject gameObject => owner.gameObject;
        protected Transform transform => owner.transform;
        protected Renderer rend => owner.rend;
        protected Animator anim => owner.animator;
        protected Collider coll => owner.coll;
        protected SlimeState(Slime owner, StateMachine<State, Slime> stateMachine) : base(owner, stateMachine)
        {
        }
    }

    private class IdleState : SlimeState
    {
        float range;
        float angle;
        LayerMask targetMask;
        LayerMask obstacleMask;

        public IdleState(Slime owner, StateMachine<State, Slime> stateMachine) : base(owner, stateMachine)
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
            if(owner.target != null)
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
    private class TraceState : SlimeState
    {
        public TraceState(Slime owner, StateMachine<State, Slime> stateMachine) : base(owner, stateMachine)
        {
        }

        public override void Enter()
        {
            owner.isAttacking = false;
            if (owner.lookRoutine != null)
                owner.StopCoroutine(owner.lookRoutine);
            owner.lookRoutine = owner.StartCoroutine(owner.LookRoutine());
        }

        public override void Exit()
        {

        }

        public override void Setup()
        {
            
        }

        public override void Transition()
        {
            if(Vector3.Distance(owner.target.transform.position, transform.position) > owner.monsterData.detectRange)
            {
                owner.target = null;
                stateMachine.ChangeState(State.Idle);
            }
        }
        
        public override void Update()
        {
            if(owner.isAttacking)
            {             
                return;
            }            
            owner.attackRoutine = owner.StartCoroutine(owner.AttackRoutine());
        }
       
    }

    private class TakeHitState : SlimeState
    {
        public TakeHitState(Slime owner, StateMachine<State, Slime> stateMachine) : base(owner, stateMachine)
        {
        }

        public override void Enter()
        {
            if (owner.takeHitRoutine != null)
                owner.StopCoroutine(owner.takeHitRoutine);
            owner.takeHitRoutine = owner.StartCoroutine(owner.TakeHitRoutine());
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
    private class DieState : SlimeState
    {
        public DieState(Slime owner, StateMachine<State, Slime> stateMachine) : base(owner, stateMachine)
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

