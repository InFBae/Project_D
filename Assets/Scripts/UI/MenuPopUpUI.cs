using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MenuPopUpUI : PopUpUI
{
    [SerializeField] BaseUI equipmentUI;
    [SerializeField] BaseUI inventoryUI;
    [SerializeField] BaseUI statsUI;
    [SerializeField] BaseUI settingUI;

    PlayerInput input;
    protected override void Awake()
    {
        base.Awake();
     
        buttons["EquipmentsButton"].onClick.AddListener(()=> equipmentUI.transform.SetAsLastSibling());
        buttons["InventoryButton"].onClick.AddListener(() => inventoryUI.transform.SetAsLastSibling());
        buttons["StatsButton"].onClick.AddListener(() => statsUI.transform.SetAsLastSibling());
        buttons["SettingsButton"].onClick.AddListener(() => settingUI.transform.SetAsLastSibling());        
        buttons["ApplyButton"].onClick.AddListener(OnApplyButton);
        buttons["RevertButton"].onClick.AddListener(OnRevertButton);
    
        input = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
    }
    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnApplyButton()
    {
        UIController.OnMenuPopUpUIClosed?.Invoke();
        CloseUI();
    }
    
    public void OnRevertButton()
    {
        UIController.OnMenuPopUpUIClosed?.Invoke();
        CloseUI();
    }

}
