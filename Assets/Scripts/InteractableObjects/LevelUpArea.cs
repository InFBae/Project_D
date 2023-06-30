using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpArea : MonoBehaviour, IInteractable
{
    Canvas canvas;

    private void Awake()
    {
        canvas = GetComponentInChildren<Canvas>();
        canvas.enabled = false;
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

    public void Interact()
    {
        Debug.Log("LevelUpInteract");
        /*MenuPopUpUI menuPopUpUI = GameManager.Resource.Load<MenuPopUpUI>("UI/MenuUI");

        if (isMenuUIClosed)
        {
            isMenuUIClosed = false;
            GameManager.UI.ShowPopUpUI(menuPopUpUI);
        }*/
    }
}
