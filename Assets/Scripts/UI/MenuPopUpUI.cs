using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MenuPopUpUI : PopUpUI
{
    private static bool isOpened = false;
    public static bool IsOpened { get { return isOpened; } }

    [SerializeField] BaseUI equipmentUI;
    [SerializeField] BaseUI inventoryUI;
    [SerializeField] BaseUI statsUI;
    [SerializeField] BaseUI settingUI;

    protected override void Awake()
    {
        base.Awake();
     
        buttons["EquipmentsButton"].onClick.AddListener(()=> equipmentUI.transform.SetAsLastSibling());
        buttons["InventoryButton"].onClick.AddListener(() => inventoryUI.transform.SetAsLastSibling());
        buttons["StatsButton"].onClick.AddListener(() => statsUI.transform.SetAsLastSibling());
        buttons["SettingsButton"].onClick.AddListener(() => settingUI.transform.SetAsLastSibling());        
        buttons["ApplyButton"].onClick.AddListener(OnApplyButton);
        buttons["RevertButton"].onClick.AddListener(OnRevertButton);
        buttons["ReturnTitleButton"].onClick.AddListener(OnReturnTitleButton);
    }

    private void OnEnable()
    {
        isOpened = true;
        Cursor.lockState = CursorLockMode.None;
    }
    private void OnDisable()
    {
        isOpened = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnApplyButton()
    {      
        CloseUI();
    }
    
    public void OnRevertButton()
    {
        CloseUI();
    }

    public void OnReturnTitleButton()
    {
        GameManager.Scene.LoadScene("GameTitleScene", "");
    }

}
