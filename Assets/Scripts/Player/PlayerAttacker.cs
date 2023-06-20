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
    private PlayerStatusData statusData;

    [SerializeField] Weapon curWeapon;
     
    private void Awake()
    {
        animator = GetComponent<Animator>();
        statusController = GetComponent<PlayerStatusController>();
        stateController = GetComponent<PlayerStateController>();
        statusData = GameManager.Resource.Load<PlayerStatusData>("Data/PlayerStatusData");
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
            // ���� ���� ��
            if (stateController.CurState == PlayerStateController.State.Attacking &&
                statusController.GetCurrentSP() >= 2 &&
                animator.GetBool("ContinuousAttack"))
            {
                animator.SetBool("IsAttacking", true);
                animator.SetBool("ContinuousAttack", false);
                curWeapon?.EnableCollider();
                statusController.DecreaseSP(2);
                // ���� �ð���ŭ ���� ����
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
                // TODO : ���� ����� ȿ��
            }
            else // �����ߵ� ����ߵ� �ƴҰ�� ���÷�ƾ ����
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
