using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.GridLayoutGroup;

public class Slime : Monster
{
    [SerializeField] Collider attackCollider;

    public enum State { Idle, Trace, Returning, Die, Size }
    StateMachine<State, Slime> stateMachine;
    
    private GameObject target;
    private bool isAttacking = false;

    protected override void Awake()
    {
        base.Awake();
        InitData();
        attackCollider.gameObject.SetActive(false);

        stateMachine = new StateMachine<State, Slime>(this);
        stateMachine.AddState(State.Idle, new IdleState(this, stateMachine));
        stateMachine.AddState(State.Trace, new TraceState(this, stateMachine));
    }

    private void Start()
    {
        stateMachine.SetUp(State.Idle);
    }
    private void Update()
    {
        stateMachine.Update();
    }
    public override void TakeHit(float damage)
    {
        curHP -= damage;
        animator.SetTrigger("gotHit");
        if(curHP < 0)
        {
            stateMachine.ChangeState(State.Die);
        }
    }

    private void InitData()
    {
        monsterData = GameManager.Resource.Load<MonsterData>("Data/SlimeData");
        curHP = monsterData.maxHP;        
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        
        if (lookRoutine != null)
        {
            StopCoroutine(lookRoutine);
        }

        if (Vector3.Distance(target.transform.position, transform.position) > monsterData.attackRange)
        {
            animator.SetTrigger("attack2");
            rb.AddForce(Vector3.up * 10f + Vector3.forward * 8f, ForceMode.Impulse);
        }
        else
        {
            animator.SetTrigger("attack1");
            yield return new WaitForSeconds(0.5f);
            attackCollider.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            attackCollider.gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(monsterData.attackCooltime);
        
        lookRoutine = StartCoroutine(LookRoutine());
        yield return new WaitForSeconds(1f);
        isAttacking = false;        
    }

    Coroutine lookRoutine;
    IEnumerator LookRoutine()
    {
        while(target != null)
        {
            Quaternion lookRotation = Quaternion.LookRotation(target.transform.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, 3 * Time.deltaTime);
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TriggerEnter");
        IHittable hittable = other.GetComponent<IHittable>();
        hittable?.TakeHit(monsterData.damage);
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
            owner.animator.SetTrigger("idleBreak");
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
            Collider[] targets = Physics.OverlapSphere(transform.position, range, targetMask);
            for (int i = 0; i < targets.Length; i++)
            {
                Vector3 dirToTarget = (targets[i].transform.position - transform.position).normalized;

                // 2. 각도
                if (Vector3.Dot(transform.forward, dirToTarget) < angle)
                    continue;

                // 3. 중간 장애물
                float distToTarget = Vector3.Distance(transform.position, targets[i].transform.position);
                if (Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
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
                stateMachine.ChangeState(State.Idle);
            }
        }

        
        public override void Update()
        {
            if(owner.isAttacking)
            {             
                return;
            }
            if (owner.lookRoutine == null)
                owner.lookRoutine = owner.StartCoroutine(owner.LookRoutine());
            
            owner.StartCoroutine(owner.AttackRoutine());
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

