using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedPotion : Item, IUsable
{
    public RedPotion()
    {
        data = GameManager.Resource.Load<ItemData>("Data/Items/RedPotionData");
    }   

    public void Use()
    {
        if (count > 0)
        {
            owner.IncreaseHP((GameManager.Data.PlayerStatusData.maxHP / 2));
            count--;
        }
        
    }
}
