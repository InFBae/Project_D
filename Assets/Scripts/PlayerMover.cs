using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] Transform lookPoint;

    private CharacterController controller;
    private Animator animator;
    private Rig rig;

    private Vector3 moveDir;
    private float ySpeed = 0;
    private bool run = false;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        rig = GetComponentInChildren<Rig>();
    }

    private void OnEnable()
    {
        moveRoutine = StartCoroutine(MoveRoutine());
        jumpRoutine = StartCoroutine(JumpRoutine());
    }

    private void OnDisable()
    {
        StopCoroutine(moveRoutine);
        StopCoroutine(jumpRoutine);
    }

    Coroutine moveRoutine;
    private IEnumerator MoveRoutine()
    {
        while (true)
        {
            if (run && animator.GetFloat("YInput") > 0.1)
            {
                controller.Move(transform.forward * moveDir.z * runSpeed * Time.deltaTime);
                controller.Move(transform.right * moveDir.x * runSpeed * Time.deltaTime);
            }
            else
            {
                controller.Move(transform.forward * moveDir.z * moveSpeed * Time.deltaTime);
                controller.Move(transform.right * moveDir.x * moveSpeed * Time.deltaTime);
            }

            animator.SetFloat("XInput", moveDir.x, 0.1f, Time.deltaTime);
            animator.SetFloat("YInput", moveDir.z, 0.1f, Time.deltaTime);

            Look();

            yield return null;
        }
    }

    private void Look()
    {
        Vector3 point = lookPoint.position;
        point.y = transform.position.y;
        transform.LookAt(point);
    }

    Coroutine jumpRoutine;
    private IEnumerator JumpRoutine()
    {
        while (true)
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;
            /*
            if (GroundCheck() && ySpeed < 0)
                ySpeed = -1;
            */
            controller.Move(Vector3.up * ySpeed * Time.deltaTime);

            if(ySpeed > 0) animator.SetBool("IsGround", false);
            else animator.SetBool("IsGround", true);

            yield return null;
        }
    }

    private void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        moveDir = new Vector3(input.x, 0, input.y);

        animator.SetBool("Move", input.sqrMagnitude > 0);
    }

    /*
    private void OnJump(InputValue value)
    {
        if (GroundCheck())
            ySpeed = jumpSpeed;
    }
    */

    private void OnRun(InputValue value)
    {
        if (animator.GetBool("Block") || !GroundCheck())
        {
            animator.SetBool("Run", false);
            return;
        }

        run = value.isPressed;

        animator.SetBool("Run", run);
        if (run )
        {
            animator.SetLayerWeight(1, 0);
            rig.weight = 0f;
        }
        else
        {
            animator.SetLayerWeight(1, 1);
            rig.weight = 1f;
        }
    }

    private bool GroundCheck()
    {
        RaycastHit hit;
        
        return Physics.SphereCast(transform.position + Vector3.up * 1f, 0.5f, Vector3.down, out hit, 0.8f);
    }

}

