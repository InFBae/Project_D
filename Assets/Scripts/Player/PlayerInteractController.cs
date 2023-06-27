using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractController : MonoBehaviour
{
    PlayerInteractor interactor;

    private void Awake()
    {
        interactor = GetComponentInChildren<PlayerInteractor>();
    }

    private void OnInteract(InputValue input)
    {
        if (interactor.interactable != null)
        {
            interactor.interactable?.Interact();
        }

    }
}
