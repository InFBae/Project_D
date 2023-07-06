using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : BaseUI, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private InventoryUI owner;
    protected override void Awake()
    {
        base.Awake();
        owner = GetComponentInParent<InventoryUI>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int inventoryIndex = transform.GetSiblingIndex() + owner.page * 32;
        if (inventoryIndex < GameManager.Data.PlayerStatusData.inventory.Count)
        {
            InventorySlotUI inventorySlotUI = GameManager.UI.ShowWindowUI<InventorySlotUI>("UI/InventorySlotUI");
            inventorySlotUI.transform.SetParent(transform.parent, false);
            inventorySlotUI.transform.position = new Vector3(transform.position.x + 150, transform.position.y);
            inventorySlotUI.index = inventoryIndex;
            inventorySlotUI.owner = owner;
        }   
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        images["Frame"].color = Color.red;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        images["Frame"].color = Color.white;
    }
}
