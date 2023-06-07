using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageAttacker : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void OnAttack()
    {
        animator.SetTrigger("Attack");
    }
}
