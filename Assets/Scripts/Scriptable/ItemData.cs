using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Data/Item")]

public class ItemData : ScriptableObject
{
    public Sprite sprite;
    public int count;
    public string instruction;
}
