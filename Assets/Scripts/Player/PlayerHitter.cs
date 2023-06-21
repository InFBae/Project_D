using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UIElements;

public class PlayerHitter : MonoBehaviour
{
    [SerializeField] float weakHitTime = 0.6f;
    [SerializeField] float strongHitTime = 0.8f;

    Animator animator;
    private PlayerStateController stateController;
    private PlayerStatusController statusController;
    private CharacterController controller;
    private PlayerMover mover;
    Rig rig;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        stateController = GetComponent<PlayerStateController>();
        statusController = GetComponent<PlayerStatusController>();
        controller = GetComponent<CharacterController>();
        mover = GetComponent<PlayerMover>();
        rig = GetComponentInChildren<Rig>();
    }

    public Coroutine hitRoutine;
    private float currentTime;
    private float hitTime;
    public IEnumerator HitRoutine(float damage)
    {
        animator.SetLayerWeight(1, 0);
        rig.weight = 0;
       
        animator.SetFloat("TakeHitDamage", damage);

        currentTime = 0;
        statusController.DecreaseHP(1);

        if (animator.GetFloat("TakeHitDamage") >= 2)
        {
            hitTime = strongHitTime;
        }
        else
        {
            hitTime = weakHitTime;
        }

        animator.SetTrigger("TakeHit");
        while (currentTime < hitTime)
        {
            currentTime += Time.deltaTime;
            controller.Move(transform.forward * -0.5f * Time.deltaTime);
            yield return null;
        }

        rig.weight = 1;
        mover.moveRoutine = mover.StartCoroutine(mover.MoveRoutine());

        stateController.CurState = (PlayerStateController.State)stateController.IsMoving();        
    }
}
