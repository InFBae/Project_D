using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class UIController : MonoBehaviour
{
    private void OnMenu(InputValue input)
    {
        if (MenuPopUpUI.IsOpened)
        {
            GameManager.UI.ClosePopUpUI();
            if (Cursor.lockState == CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            return;
        }
        if (!MenuPopUpUI.IsOpened && GameManager.Data.PlayerState == PlayerStateController.State.Idle)
        {
            GameManager.UI.ShowPopUpUI<MenuPopUpUI>("UI/MenuUI");
        }
        
    }
}
