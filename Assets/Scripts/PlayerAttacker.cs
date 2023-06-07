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
        animator.SetTrigger("Attack");
    }

    private void OnBlock(InputValue inputValue)
    {
        bool isBlocking = inputValue.isPressed;
        animator.SetBool("Block", isBlocking);
    }
}
