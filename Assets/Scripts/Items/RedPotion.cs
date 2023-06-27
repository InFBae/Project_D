using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedPotion : Item
{
    private void Awake()
    {
        data = GameManager.Resource.Load<ItemData>("Data/Items/RedPotionData");
    }
}
