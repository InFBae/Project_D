using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedPotion : Item, IUsable
{
    public RedPotion()
    {
        name = "RedPotion";
        data = GameManager.Resource.Load<ItemData>("Data/Items/RedPotionData");
    }   

    public void Use()
    {
        if (count > 0)
        {
            GameManager.Sound.Play("PotionDrink");
            GameManager.Data.IncreaseHP((GameManager.Data.PlayerStatusData.MaxHP / 2));
            count--;
            if (count == 0)
            {
                GameManager.Data.PlayerStatusData.inventory.Remove(this);
            }
        }
    }
}
