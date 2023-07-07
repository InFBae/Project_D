using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class PlayerAttacker : MonoBehaviour
{
    private Animator animator;
    private PlayerStatusController statusController;
    private PlayerStateController stateController;
    private Dictionary<IHittable, float> hitTable;
    private Rig rig;

    [SerializeField] Weapon curWeapon;
     

    private void Awake()
    {
        animator = GetComponent<Animator>();
        statusController = GetComponent<PlayerStatusController>();
        stateController = GetComponent<PlayerStateController>();
        hitTable = new Dictionary<IHittable, float>();
        rig = GetComponentInChildren<Rig>();

        curWeapon.hitTable = hitTable;
        curWeapon.owner = stateController;
    }

    private void Start()
    {
        attackTime = curWeapon.GetAttackCoolTime();
    }

    public bool continuousAttack;
    private float attackTime;

    public Coroutine attackRoutine;
    public IEnumerator AttackRoutine()
    {        
        while (true)
        {
            // 공격 중일 때
            if (stateController.CurState == PlayerStateController.State.Attacking &&
                statusController.GetCurrentSP() >= 15f &&
                animator.GetBool("ContinuousAttack"))
            {
                animator.SetBool("IsAttacking", true);
                animator.SetBool("ContinuousAttack", false);
                statusController.DecreaseSP(15f);
                
                // hitTable 초기화
                hitTable.Clear();
                curWeapon?.EnableCollider();
                
                // 공격 시간만큼 공격 실행
                yield return new WaitForSeconds(attackTime/2f);
                PlayWeaponSwingSound();
                yield return new WaitForSeconds(attackTime / 2f);

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

    public IEnumerator StrongAttackRoutine()
    {
        if ( statusController.GetCurrentSP() >= 30f)
        {
            rig.weight = 0f;
            animator.SetTrigger("StrongAttack");
            animator.SetBool("IsAttacking", true);
            statusController.DecreaseSP(30f);

            // hitTable 초기화
            hitTable.Clear();

            yield return new WaitForSeconds(0.5f);
            curWeapon?.EnableCollider();
            yield return new WaitForSeconds(0.5f);           
            curWeapon?.DisableCollider();
            yield return new WaitForSeconds(1f);

            animator.SetBool("IsAttacking", false);
            rig.weight = 1f;            
        }
        stateController.CurState = (PlayerStateController.State)stateController.IsMoving();
    }

    public void PlayWeaponSwingSound()
    {
        GameManager.Sound.Play("MetalWeaponSwing");
    }

}
