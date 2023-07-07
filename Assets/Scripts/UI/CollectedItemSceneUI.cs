using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectedItemSceneUI : SceneUI
{
    protected override void Awake()
    {
        base.Awake();
    }

    public void SetUI(Item item, int count)
    {
        images["CollectedItemIcon"].sprite = item.Data.sprite;
        texts["CollectedItemCount"].text = count.ToString();
        texts["CollectedItemName"].text = item.Data.itemName;
    }
}
