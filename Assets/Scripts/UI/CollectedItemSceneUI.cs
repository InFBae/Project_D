using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using static UnityEditor.Progress;

public class CollectedItemSceneUI : SceneUI
{
    protected override void Awake()
    {
        base.Awake();
    }

    public void SetUI(Item item)
    {
        images["CollectedItemIcon"].sprite = item.Data.sprite;
        texts["CollectedItemCount"].text = item.Count.ToString();
        texts["CollectedItemName"].text = item.Data.itemName;
    }
}
