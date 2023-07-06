using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluePotion : Item, IUsable
{
    public BluePotion()
    {
        name = "BluePotion";
        data = GameManager.Resource.Load<ItemData>("Data/Items/BluePotionData");
    }
    public void Use()
    {
        if (count > 0)
        {
            GameManager.Sound.Play("PotionDrink");
            GameManager.Data.IncreaseHP(50);
            count--;
            if (count == 0)
            {
                GameManager.Data.PlayerStatusData.inventory.Remove(this);
            }
        }

    }
}
