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
            // ���� ���� ��
            if (stateController.CurState == PlayerStateController.State.Attacking &&
                statusController.GetCurrentSP() >= 2 &&
                animator.GetBool("ContinuousAttack"))
            {
                animator.SetBool("IsAttacking", true);
                animator.SetBool("ContinuousAttack", false);
                statusController.DecreaseSP(2);

                // hitTable �ʱ�ȭ
                hitTable.Clear();
                curWeapon?.EnableCollider();
                
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
