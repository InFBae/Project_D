using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    public IInteractable interactable;

    private void OnTriggerEnter(Collider other)
    {
        interactable = other.GetComponent<IInteractable>();
        interactable?.Enter();            
    }

    private void OnTriggerExit(Collider other)
    {
        interactable?.Exit();
        interactable = null;
    }
}
