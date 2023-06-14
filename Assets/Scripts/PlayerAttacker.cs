using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttacker : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
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
}
