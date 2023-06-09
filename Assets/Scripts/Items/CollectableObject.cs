using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableObject : MonoBehaviour, ICollectable
{
    [SerializeField] ItemData data;
    public int count = 1;

    public void Collect(PlayerStatusController playerStatusController)
    {
        if (data.isStackable)
        {
            foreach (Item item in GameManager.Data.PlayerStatusData.inventory)
            {
                if (item.Data.isStackable)
                {
                    if (data.itemName == item.Data.itemName)
                    {
                        GameManager.UI.FloatMessage(item, count);
                        item.SetCount(item.Count + count);
                        StatusInfoSceneUI.OnQuickSlotChanged?.Invoke();
                        return;
                    }
                }
            }
        }
        if (data.itemName == "RedPotion")
        {
            RedPotion redPotion = new RedPotion();         
            redPotion.SetCount(count);
            GameManager.Data.PlayerStatusData.inventory.Add(redPotion);
            GameManager.UI.FloatMessage(redPotion, count);
        }
        else if (data.itemName == "BluePotion")
        {
            BluePotion bluePotion = new BluePotion();
            bluePotion.SetCount(count);
            GameManager.Data.PlayerStatusData.inventory.Add(bluePotion);
            GameManager.UI.FloatMessage(bluePotion, count);
        }

        /*else if (data.itemName == "OHSword")
        {
            GameManager.Data.PlayerStatusData.inventory.Add(new OHSword());
        }
        else if (data.itemName == "Shield")
        {
            GameManager.Data.PlayerStatusData.inventory.Add(new Shield());
        }*/
    }

    public void SetCount(int count)
    {
        this.count = count;
    }
}
