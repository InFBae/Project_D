using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class UIController : MonoBehaviour
{
    private bool isMenuUIClosed = true;

    public static UnityAction OnMenuPopUpUIClosed;

    private void OnEnable()
    {
        OnMenuPopUpUIClosed += SetMenuBool;
    }

    private void OnDisable()
    {
        OnMenuPopUpUIClosed -= SetMenuBool;
    }

    private void SetMenuBool()
    {
        isMenuUIClosed = true;
    }

    private void OnMenu(InputValue input)
    {        
        MenuPopUpUI menuPopUpUI = GameManager.Resource.Load<MenuPopUpUI>("UI/MenuUI");
       
        if (isMenuUIClosed)
        {
            isMenuUIClosed = false;
            GameManager.UI.ShowPopUpUI(menuPopUpUI);
        }       
    }
}
