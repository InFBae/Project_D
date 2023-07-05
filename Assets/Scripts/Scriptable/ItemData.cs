using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Data/Item")]

public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite sprite;
    public string instruction;
    public bool isStackable;
    public bool isUsable;

}
