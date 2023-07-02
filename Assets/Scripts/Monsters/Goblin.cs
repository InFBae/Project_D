using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using static UnityEngine.UI.GridLayoutGroup;

public class Goblin : Monster
{
    [SerializeField] Collider attackCollider;
    public enum State { Idle, Trace, Attack, Return, TakeHit, Die, Size }
    StateMachine<State, Goblin> stateMachine;

    private bool[] skillAvailability;
    private float[] skillCoolTime;

    [SerializeField] public Transform spawnPoint;
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
    public override void TakeHit(float damage, GameObject attacker, IHittable.HitType hitType)
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);
        if (lookRoutine != null)
            StopCoroutine(lookRoutine);
        if (attackRoutine != null)
            StopCoroutine(attackRoutine);
        if (takeHitRoutine != null)
            StopCoroutine(takeHitRoutine);
        if (returnRoutine != null)
            StopCoroutine(returnRoutine);

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

        DropItem();
        GameManager.Data.CurEXP += 50;
        GameManager.Pool.Release(gameObject, 5f);
    }

    public override void DropItem()
    {
        GameObject bluePotion = GameManager.Resource.Load<GameObject>("Item/BluePotion");
        float dropRate = 100f;
        if (Random.Range(0, 10000) < dropRate * 100)
        {
            GameManager.Resource.Instantiate<GameObject>(bluePotion, transform.position, transform.rotation);
        }
    }

    public void Regen()
    {
        rb.isKinematic = false;
        coll.enabled = true;
        stateMachine.SetUp(State.Idle);
        for(int i = 0; i < skillAvailability.Length; i++)
        {
            skillAvailability[i] = true;
        }
        CurHP = MaxHP;
    }
    private void InitData()
    {
        monsterData = GameManager.Resource.Load<MonsterData>("Data/Monsters/GoblinData");
        CurHP = monsterData.maxHP;

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

        if (skillAvailability[0] && Vector3.Distance(target.transform.position, transform.position + (Vector3.up * 1)) >= skill1Range)
        {
            animator.SetTrigger("attack1");
            yield return new WaitForSeconds(1.6f);
            skillAvailability[0] = false;
            StartCoroutine(TimerRoutine(skillCoolTime[0], 0));
        }
        else if (skillAvailability[1] && Vector3.Distance(target.transform.position, transform.position + (Vector3.up * 1)) < 3)
        {
            animator.SetTrigger("attack2");
            yield return new WaitForSeconds(2.66f);
            skillAvailability[1] = false;
            StartCoroutine(TimerRoutine(skillCoolTime[1], 1));
        }
        else if (skillAvailability[2] && Vector3.Distance(target.transform.position, transform.position + (Vector3.up * 1)) < 3)
        {
            animator.SetTrigger("attack3");
            yield return new WaitForSeconds(2.2f);
            skillAvailability[2] = false;
            StartCoroutine(TimerRoutine(skillCoolTime[2], 2));
        }
        else if (skillAvailability[3] && Vector3.Distance(target.transform.position, transform.position + (Vector3.up * 1)) < 3)
        {
            animator.SetTrigger("attack4");
            yield return new WaitForSeconds(2.8f);
            skillAvailability[3] = false;
            StartCoroutine(TimerRoutine(skillCoolTime[3], 3));
        }

        attackCollider.enabled = false;

        if (Vector3.Distance(target.transform.position, transform.position + (Vector3.up * 1)) > monsterData.attackRange)
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
        yield return new WaitForSeconds(coolTime);
        
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
                hittable.TakeHit(monsterData.damage, gameObject, IHittable.HitType.Weak);
        }
    }

    Coroutine takeHitRoutine;
    IEnumerator TakeHitRoutine()
    {

        animator.SetTrigger("weakHit");

        yield return new WaitForSeconds(0.6f);

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

    float angle;
    LayerMask targetMask;
    LayerMask obstacleMask;
    public void FindTarget()
    {
        // 1. 범위
        Collider[] targets = Physics.OverlapSphere(transform.position + (Vector3.up * 1), monsterData.detectRange, targetMask);
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
            owner.target = null;
            if (owner.moveRoutine != null)
                owner.StopCoroutine(owner.moveRoutine);
            if (owner.lookRoutine != null)
                owner.StopCoroutine(owner.lookRoutine);
            if (owner.attackRoutine != null)
                owner.StopCoroutine(owner.attackRoutine);
            if (owner.takeHitRoutine != null)
                owner.StopCoroutine(owner.takeHitRoutine);
            if (owner.returnRoutine != null)
                owner.StopCoroutine(owner.returnRoutine);
            owner.animator.SetBool("move", false);
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
            //Debug.Log("EnterTrace");
            if (owner.lookRoutine != null)
                owner.StopCoroutine(owner.lookRoutine);
            owner.lookRoutine = owner.StartCoroutine(owner.LookRoutine());
            owner.animator.SetBool("move", true);
        }

        public override void Exit()
        {
            //Debug.Log("ExitTrace");
            owner.animator.SetBool("move", false);
        }

        public override void Setup()
        {
        }

        public override void Transition()
        {
            if (Vector3.Distance(owner.target.transform.position, transform.position + (Vector3.up * 1)) < owner.monsterData.attackRange)
            {                
                stateMachine.ChangeState(State.Attack);
            }
            else if (Vector3.Distance(owner.target.transform.position, transform.position + (Vector3.up * 1)) > owner.monsterData.detectRange + 1)
            {
                owner.target = null;
                stateMachine.ChangeState(State.Return);
            }
            else if (Vector3.Distance(owner.spawnPoint.transform.position, transform.position) > owner.monsterData.detectRange * 2)
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
            //Debug.Log("EnterAttack");
        }

        public override void Exit()
        {
            if(owner.attackRoutine != null) 
                owner.StopCoroutine(owner.attackRoutine);
            owner.isAttacking = false;
            if (owner.moveRoutine != null)
                owner.StopCoroutine(owner.moveRoutine);
        }

        public override void Setup()
        {
        }

        public override void Transition()
        {
            if(!owner.isAttacking && Vector3.Distance(owner.target.transform.position, transform.position + (Vector3.up * 1)) > owner.monsterData.attackRange)
            {
                stateMachine.ChangeState(State.Trace);
            }
            if (!owner.isAttacking && Vector3.Distance(owner.spawnPoint.transform.position, transform.position + (Vector3.up * 1)) > owner.monsterData.detectRange * 2)
            {
                owner.target = null;
                stateMachine.ChangeState(State.Return);
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
            //Debug.Log("EnterReturn");
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
            //Debug.Log("EnterTakeHit");
            owner.animator.SetBool("move", false);
            if (owner.takeHitRoutine != null)
                owner.StopCoroutine(owner.takeHitRoutine);
            owner.StartCoroutine(owner.TakeHitRoutine());
        }

        public override void Exit()
        {
            //Debug.Log("ExitTakeHit");
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
            owner.animator.SetBool("move", false);
            owner.Die();
        }

        public override void Exit() { }

        public override void Setup() { }

        public override void Transition() { }

        public override void Update() { }
    }
    #endregion
}