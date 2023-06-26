using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    IInteractable interactable;

    private void OnTriggerEnter(Collider other)
    {
        interactable = other.GetComponent<IInteractable>();
        if(interactable is Door)
        {
            Door door = interactable as Door;
            door.EnterDoor();
        }
            
    }

    private void OnTriggerExit(Collider other)
    {
        if (interactable is Door)
        {
            Door door = interactable as Door;
            door.ExitDoor();
        }
        interactable = null;
    }

    private void OnInteract()
    {
        if(interactable != null)
        {
            interactable.Interact();
        }
    }
}
