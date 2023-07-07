using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventorySlotUI : WindowUI
{
    public int index;
    public InventoryUI owner;

    protected override void Awake()
    {
        base.Awake();

        buttons["UseButton"].onClick.AddListener(OnUseButton);
        buttons["DropButton"].onClick.AddListener(OnDropButton);
        buttons["QuickSlotButton"].onClick.AddListener(OnQuickSlotButton);
        //buttons["CloseButton"].onClick.AddListener(()=> CloseUI());
    }

    public void OnUseButton()
    {
        Item item = GameManager.Data.PlayerStatusData.inventory[index];
        if (item is IUsable && item.Count > 0)
        {
            (item as IUsable).Use();
            StatusInfoSceneUI.OnQuickSlotChanged();
        }
        owner.UpdateUI();
        CloseUI();
    }

    public void OnDropButton()
    {
        Item item = GameManager.Data.PlayerStatusData.inventory[index];
        Transform playerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        CollectableObject dropItem = GameManager.Resource.Instantiate<CollectableObject>($"Item/{item.Data.itemName}");
        dropItem.count = item.Count;

        dropItem.transform.position = playerPosition.position + playerPosition.forward * 2;
        GameManager.Data.PlayerStatusData.inventory.RemoveAt(index);
        CloseUI();
        owner.UpdateUI();
    }

    public void OnQuickSlotButton()
    {
        Item item = GameManager.Data.PlayerStatusData.inventory[index];
        if( item is IUsable && item.Count > 0)
        {
            GameManager.Data.PlayerStatusData.quickItemList.Add(item);
            StatusInfoSceneUI.OnQuickSlotChanged?.Invoke();
        }
        owner.UpdateUI();
        CloseUI();
    }

}
