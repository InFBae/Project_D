using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluePotion : Item
{
    public BluePotion()
    {
        data = GameManager.Resource.Load<ItemData>("Data/Items/BluePotionData");
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
