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
                        item.IncreaseCount(count);
                        return;
                    }
                }
            }
        }
        if (data.itemName == "RedPotion")
        {
            RedPotion redPotion = new RedPotion();
            redPotion.owner = playerStatusController;            
            redPotion.IncreaseCount(count);
            GameManager.Data.PlayerStatusData.inventory.Add(redPotion);
            GameManager.UI.FloatMessage(redPotion, count);
        }
        else if (data.itemName == "BluePotion")
        {
            BluePotion bluePotion = new BluePotion();
            bluePotion.owner = playerStatusController;
            bluePotion.IncreaseCount(count);
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
