using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttacker : MonoBehaviour
{
    private Animator animator;
    private PlayerStatusController statusController;
    private PlayerStateController stateController;
    private Dictionary<IHittable, float> hitTable;

    [SerializeField] Weapon curWeapon;
     

    private void Awake()
    {
        animator = GetComponent<Animator>();
        statusController = GetComponent<PlayerStatusController>();
        stateController = GetComponent<PlayerStateController>();
        hitTable = new Dictionary<IHittable, float>();

        curWeapon.hitTable = hitTable;
    }

    private void Start()
    {
        attackTime = curWeapon.GetAttackCoolTime();
    }

    public bool continuousAttack;
    private float currentAttackTime;
    private float attackTime;

    public Coroutine attackRoutine;
    public IEnumerator AttackRoutine()
    {        
        while (true)
        {
            // 공격 중일 때
            if (stateController.CurState == PlayerStateController.State.Attacking &&
                statusController.GetCurrentSP() >= 2 &&
                animator.GetBool("ContinuousAttack"))
            {
                animator.SetBool("IsAttacking", true);
                animator.SetBool("ContinuousAttack", false);
                statusController.DecreaseSP(2);

                // hitTable 초기화
                hitTable.Clear();
                curWeapon?.EnableCollider();
                
                // 공격 시간만큼 공격 실행
                currentAttackTime = 0;
                while (currentAttackTime < attackTime)
                {
                    currentAttackTime += Time.deltaTime;
                    yield return null;
                }
                animator.SetBool("IsAttacking", false);
                curWeapon?.DisableCollider();
                if (!animator.GetBool("ContinuousAttack"))
                {
                    stateController.CurState = (PlayerStateController.State)stateController.IsMoving();
                }
            }
            else if (stateController.CurState == PlayerStateController.State.Blocking)
            {
                // TODO : 방패 사용중 효과
            }
            else // 공격중도 방어중도 아닐경우 어택루틴 종료
            {
                animator.SetLayerWeight(1, 0);
                animator.SetBool("ContinuousAttack", false);
                stateController.CurState = (PlayerStateController.State)stateController.IsMoving();
                yield break;
            }
           
            yield return null;            
        }     
    }



}
