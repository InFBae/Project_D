using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] string start;
    [SerializeField] string dest;
    Canvas canvas;

    private void Awake()
    {
        canvas = GetComponentInChildren<Canvas>();
        canvas.enabled = false;
    }

    public void Interact()
    {
        GameManager.Sound.Play("DoorOpen");
        GameManager.Scene.LoadScene(dest, start);
    }

    public void Enter()
    {
        if (canvas != null)
        {
            canvas.enabled = true;
        }
    }

    public void Exit()
    {
        if (canvas != null)
        {
            canvas.enabled = false;
        }
    }
}
