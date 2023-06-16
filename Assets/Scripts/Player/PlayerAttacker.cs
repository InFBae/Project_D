using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttacker : MonoBehaviour
{
    private Animator animator;
    private PlayerStatusController statusController;
    private PlayerStatusData statusData;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        statusController = GetComponent<PlayerStatusController>();
        statusData = GameManager.Resource.Load<PlayerStatusData>("Data/PlayerStatusData");
    }

    private void Start()
    {
        attackCooltime = statusData.weaponData.attackCooltime;
    }
    private void OnEnable()
    {
        StartCoroutine(AttackRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnAttack(InputValue inputValue)
    {
        if (animator.GetBool("Run") || animator.GetBool("Block"))
        {
            animator.ResetTrigger("Attack");
            return;
        }        
        animator.SetTrigger("Attack");
    }
    private void OnBlock(InputValue inputValue)
    {
        if (animator.GetBool("Run"))
        {
            animator.SetBool("Block", false);
            return;
        }
        bool isBlocking = inputValue.isPressed;
        animator.SetBool("Block", isBlocking);
    }

    private bool isAttacking;
    private float currentCooltime;
    private float attackCooltime;
    IEnumerator AttackRoutine()
    {
        while (true)
        {
            if (isAttacking)
            {            
                yield return null;
                continue;
            }

            currentCooltime = 0;
            yield return new WaitForSeconds(attackCooltime);

            
        }     
    }

}
