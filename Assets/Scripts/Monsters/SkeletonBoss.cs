using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class SkeletonBoss : Monster
{
    [SerializeField] Collider leftWeapon;
    [SerializeField] Collider rightWeapon;

    public enum State { Idle, Trace, Attack, Die}
    StateMachine<State, SkeletonBoss> stateMachine;

    [SerializeField] float skillRange = 5;
    private bool[] skillAvailability;
    private float[] skillCoolTime;

    Canvas canvas;
    [SerializeField] public Transform spawnPoint;

    IHittable.HitType attackType = IHittable.HitType.Strong;

    protected override void Awake()
    {
        base.Awake();
        InitData();

        DisableWeaponCollider();

        canvas = GetComponentInChildren<Canvas>();
        canvas.enabled = false;

        stateMachine = new StateMachine<State, SkeletonBoss>(this);
        stateMachine.AddState(State.Idle, new IdleState(this, stateMachine));
        stateMachine.AddState(State.Trace, new TraceState(this, stateMachine));
        stateMachine.AddState(State.Attack, new AttackState(this, stateMachine));
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
        CurHP -= damage;

        if (CurHP <= 0)
        {
            stateMachine.ChangeState(State.Die);
        }
        else if (CurHP / MaxHP < 0.3f)
        {
            // Buff 한번만
        }
        else
        {
            target = attacker;
        }
    }

    public override void Die()
    {
        StopAllCoroutines();
        rb.isKinematic = true;
        coll.enabled = false;

        DropItem();
        GameManager.Data.CurEXP += 1000;
        GameManager.Pool.Release(gameObject, 5f);
    }

    public override void DropItem()
    {
        //GameObject bluePotion = GameManager.Resource.Load<GameObject>("Item/BluePotion");
        float dropRate = 100f;
        if (Random.Range(0, 10000) < dropRate * 100)
        {
            //GameManager.Resource.Instantiate<GameObject>(bluePotion, transform.position, transform.rotation);
        }
    }

    public void Regen()
    {
        rb.isKinematic = false;
        coll.enabled = true;
        stateMachine.SetUp(State.Idle);
        for (int i = 0; i < skillAvailability.Length; i++)
        {
            skillAvailability[i] = true;
        }
        CurHP = MaxHP;
    }
    private void InitData()
    {
        monsterData = GameManager.Resource.Load<MonsterData>("Data/Monsters/SkeletonBossData");
        CurHP = monsterData.maxHP;

        targetMask = (1 << LayerMask.NameToLayer("Player"));
        obstacleMask = (1 << LayerMask.NameToLayer("Environment"));

        InitSkillData();
    }

    private void InitSkillData()
    {
        skillAvailability = new bool[3];
        for (int i = 0; i < skillAvailability.Length; i++)
        {
            skillAvailability[i] = true;
        }
        skillCoolTime = new float[3];
        skillCoolTime[0] = 8;
        skillCoolTime[1] = 8;
        skillCoolTime[2] = 8;
    }
    private bool isSkillTurn = true;
    Coroutine attackRoutine;
    IEnumerator AttackRoutine()
    {       
        hitTable.Clear();

        int sequence;

        if (isSkillTurn)
        {
            sequence = RandomSkill();

            if (sequence != -1)
            {
                switch (sequence)
                {
                    case 1:
                        animator.SetTrigger("Skill1");
                        yield return new WaitForSeconds(3.2f);
                        skillAvailability[0] = false;
                        StartCoroutine(TimerRoutine(skillCoolTime[0], 0));
                        break;
                    case 2:
                        animator.SetTrigger("Skill2");
                        yield return new WaitForSeconds(4f);
                        skillAvailability[1] = false;
                        StartCoroutine(TimerRoutine(skillCoolTime[1], 1));
                        break;
                    case 3:
                        animator.SetTrigger("Skill3");
                        yield return new WaitForSeconds(5.5f);
                        skillAvailability[2] = false;
                        StartCoroutine(TimerRoutine(skillCoolTime[2], 2));
                        break;
                }
                yield return new WaitForSeconds(1.5f);
                isSkillTurn = false;
            }
        }
        else
        {
            sequence = RandomAttack();
            switch (sequence)
            {
                case 1:
                    animator.SetTrigger("Attack1");
                    yield return new WaitForSeconds(2f);
                    break;
                case 2:
                    animator.SetTrigger("Attack2");
                    yield return new WaitForSeconds(2f);
                    break;
                case 3:
                    animator.SetTrigger("Attack3");
                    yield return new WaitForSeconds(2f);
                    break;
                case 4:
                    animator.SetTrigger("Attack4");
                    yield return new WaitForSeconds(1.5f);
                    break;
            }
            isSkillTurn = true;
        } 

        stateMachine.ChangeState(State.Trace);       
    }

    IEnumerator TimerRoutine(float coolTime, int index)
    {
        yield return new WaitForSeconds(coolTime);

        skillAvailability[index] = true;
    }

    Coroutine traceRoutine;
    IEnumerator TraceRoutine()
    {
        while (target != null)
        {
            Quaternion lookRotation = Quaternion.LookRotation(target.transform.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, 10 * Time.deltaTime);
            rb.MovePosition(transform.position + transform.forward * monsterData.speed * Time.deltaTime);

            Vector3 dirToTarget = (target.transform.position - transform.position).normalized;
            if (Vector3.Dot(transform.forward, dirToTarget) > 0.95 && Vector3.Distance(transform.position, target.transform.position) <= skillRange)
            {
                stateMachine.ChangeState(State.Attack);
                yield break;
            } 
            
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IHittable hittable = other.GetComponent<IHittable>();
        if (hittable != null)
        {
            if (hitTable.TryAdd(hittable, monsterData.damage))
            {
                float damage = monsterData.damage;
                if (attackType == IHittable.HitType.Strong)
                {
                    damage *= 2;
                }
                hittable.TakeHit(damage, gameObject, attackType);
            }

        }
    }

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
            if (Vector3.Dot(transform.forward, dirToTarget) < 0.5) // Cos60도 > 결과 : 120도 시야각
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

    private int RandomSkill()
    {
        List<int> available = new List<int>();
        for (int i = 0; i < skillAvailability.Length; i++)
        {
            if (skillAvailability[i])
            {
                available.Add(i + 1);
            }
        }
        if (available.Count == 0)
            return -1;
        
        int random = Random.Range(0, available.Count);
        return available[random];
    }

    private int RandomAttack()
    {
        return Random.Range(1, 5);
    }

    private void EnableWeaponCollider()
    {
        leftWeapon.enabled = true;
        rightWeapon.enabled = true;
    }

    private void DisableWeaponCollider()
    {
        leftWeapon.enabled = false;
        rightWeapon.enabled = false;
    }

    #region SkeletonBossState
    private abstract class SkeletonBossState : StateBase<State, SkeletonBoss>
    {
        protected SkeletonBossState(SkeletonBoss owner, StateMachine<State, SkeletonBoss> stateMachine) : base(owner, stateMachine) { }
    }

    private class IdleState : SkeletonBossState
    {
        public IdleState(SkeletonBoss owner, StateMachine<State, SkeletonBoss> stateMachine) : base(owner, stateMachine) { }
        public override void Enter()
        {
            owner.animator.SetInteger("CurState", 0);
            owner.canvas.enabled = false;
        }
        public override void Exit() { }
        public override void Setup() { }
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

    private class TraceState : SkeletonBossState
    {
        public TraceState(SkeletonBoss owner, StateMachine<State, SkeletonBoss> stateMachine) : base(owner, stateMachine) { }
        public override void Enter()
        {
            owner.animator.SetInteger("CurState", 1);
            owner.canvas.enabled = true;
            if (owner.traceRoutine != null)
                owner.StopCoroutine(owner.traceRoutine);
            owner.traceRoutine = owner.StartCoroutine(owner.TraceRoutine());
        }
        public override void Exit() 
        {
            if (owner.traceRoutine != null)
                owner.StopCoroutine(owner.traceRoutine);
        }
        public override void Setup() { }
        public override void Transition() { }
        public override void Update() { }
    }

    private class AttackState : SkeletonBossState
    {
        public AttackState(SkeletonBoss owner, StateMachine<State, SkeletonBoss> stateMachine) : base(owner, stateMachine) { }
        public override void Enter()
        {
            owner.animator.SetInteger("CurState", 2);
            if (owner.attackRoutine != null)
                owner.StopCoroutine(owner.attackRoutine);
            owner.attackRoutine = owner.StartCoroutine(owner.AttackRoutine());
        }
        public override void Exit()
        {
            if (owner.attackRoutine != null)
                owner.StopCoroutine(owner.attackRoutine);
        }
        public override void Setup() { }
        public override void Transition() { }
        public override void Update() { }
    }

    private class DieState : SkeletonBossState
    {
        public DieState(SkeletonBoss owner, StateMachine<State, SkeletonBoss> stateMachine) : base(owner, stateMachine) { }
        public override void Enter()
        {
            owner.animator.SetInteger("CurState", 3);
            owner.Die();
        }
        public override void Exit() { }
        public override void Setup() { }
        public override void Transition() { }
        public override void Update() { }
    }

    #endregion
}
