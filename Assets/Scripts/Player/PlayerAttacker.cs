using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttacker : MonoBehaviour
{
    private Animator animator;
    private PlayerStatusController statusController;
    private PlayerStateController stateController;
    private PlayerStatusData statusData;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        statusController = GetComponent<PlayerStatusController>();
        stateController = GetComponent<PlayerStateController>();
        statusData = GameManager.Resource.Load<PlayerStatusData>("Data/PlayerStatusData");
    }

    private void Start()
    {
        attackCooltime = statusData.weaponData.attackCooltime;
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
