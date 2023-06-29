using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedPotion : Item, IUsable
{
    PlayerStatusController owner;
    private void Awake()
    {
        data = GameManager.Resource.Load<ItemData>("Data/Items/RedPotionData");
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
        GameObject redPotion = new GameObject();
        redPotion.AddComponent<RedPotion>();
        GameManager.Data.PlayerStatusData.inventory.Add(redPotion.GetComponent<RedPotion>());
        GameManager.Resource.Destroy(this.gameObject);

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
