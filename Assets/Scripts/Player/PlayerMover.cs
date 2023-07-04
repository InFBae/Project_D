using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

[RequireComponent(typeof(CharacterController))]
public class PlayerMover : MonoBehaviour
{
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float rollDistance;
    [SerializeField] Transform lookPoint;

    private CharacterController controller;
    private Animator animator;
    private Rig rig;
    private PlayerStatusController statusController;
    private PlayerStateController stateController;

    private float ySpeed = 0;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        rig = GetComponentInChildren<Rig>();
        statusController = GetComponent<PlayerStatusController>();
        stateController = GetComponent<PlayerStateController>();
    }

    public void StartRoutines()
    {
        moveRoutine = StartCoroutine(MoveRoutine());
        fallRoutine = StartCoroutine(FallRoutine());
        lookRoutine = StartCoroutine(LookRoutine());
    }

    public void StopRoutines()
    {
        StopCoroutine(moveRoutine);
        StopCoroutine(fallRoutine);
        StopCoroutine(lookRoutine);
    }

    public Coroutine moveRoutine;
    public IEnumerator MoveRoutine()
    {
        while (true)
        {
            if (stateController.CurState == PlayerStateController.State.Running)
            {
                animator.SetLayerWeight(1, 0);
                rig.weight = 0f;

                if (statusController.GetCurrentSP() < 1 * Time.deltaTime)
                {
                    stateController.CurState = PlayerStateController.State.Walking;
                }

                statusController.DecreaseSP(1 * Time.deltaTime);
                controller.Move(transform.forward * stateController.MoveDir.z * runSpeed * Time.deltaTime);
                controller.Move(transform.right * stateController.MoveDir.x * runSpeed * Time.deltaTime);
            }
            else if(stateController.CurState == PlayerStateController.State.Walking ||
                stateController.CurState == PlayerStateController.State.Attacking ||
                stateController.CurState == PlayerStateController.State.Blocking)
            {
                rig.weight = 1f;
                controller.Move(transform.forward * stateController.MoveDir.z * walkSpeed * Time.deltaTime);
                controller.Move(transform.right * stateController.MoveDir.x * walkSpeed * Time.deltaTime);
            }
            else
            {
                rig.weight = 0f;
                yield break;
            }

            yield return null;
        }
    }

    // AimPoint 바라보도록 회전

    public Coroutine lookRoutine;

    public IEnumerator LookRoutine()
    {
        while (true)
        {
            Vector3 point = lookPoint.position;
            point.y = transform.position.y;
            transform.LookAt(point);

            yield return null;
        }
    }
    public void Look()
    {
        Vector3 point = lookPoint.position;
        point.y = transform.position.y;
        transform.LookAt(point);
    }

    // 캐릭터가 중력을 받도록 설정
    public Coroutine fallRoutine;
    public IEnumerator FallRoutine()
    {
        while (true)
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;
            
            if (stateController.CurState != PlayerStateController.State.Falling)
                ySpeed = -10;
            
            controller.Move(Vector3.up * ySpeed * Time.deltaTime);

            yield return null;
        }
    }

    public IEnumerator LandRollRoutine()
    {
        float landRollTime = 0.7f;
        float curTime = 0f;

        Vector3 rollDir = (transform.forward * stateController.MoveDir.z +
            transform.right * stateController.MoveDir.x + 
            transform.position);
        transform.LookAt(rollDir);

        animator.SetLayerWeight(1, 0);
        rig.weight = 0f;

        statusController.DecreaseSP(2);
        while (curTime < landRollTime)
        {
            curTime += Time.deltaTime;
            controller.Move(transform.forward * rollDistance * Time.deltaTime);
            yield return null;
        }

        rig.weight = 1f;
        moveRoutine = StartCoroutine(MoveRoutine());
        lookRoutine = StartCoroutine(LookRoutine());

        stateController.CurState = (PlayerStateController.State)stateController.IsMoving();
    }

}

