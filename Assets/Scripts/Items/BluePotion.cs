using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluePotion : Item
{
    PlayerStatusController owner;
    private void Awake()
    {
        data = GameManager.Resource.Load<ItemData>("Data/Items/BluePotionData");
    }

    public override void Collect()
    {
        foreach (Item item in GameManager.Data.PlayerStatusData.inventory)
        {
            if (item is IUsable)
            {
                if(data.itemName == item.Data.itemName)
                {
                    item.IncreaseCount(count);
                    GameManager.Resource.Destroy(this.gameObject);
                    return;
                }
            }
        }
        GameManager.Data.PlayerStatusData.inventory.Add(this);
        GameManager.Resource.Destroy(this.gameObject);
    }
    public void Use()
    {
        if (count > 0)
        {
            owner.IncreaseHP(50);
            count--;
        }

    }
}
