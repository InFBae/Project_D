using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item
{
    public ItemData data;
    public int count;
    public string name;

    public ItemData Data { get { return data; } }
    public int Count { get { return count; } }
    public string Name { get { return name; } } 

    public void SetCount(int num)
    {
        count = num;
    }

    public void SetData(string name)
    {
        data = GameManager.Resource.Load<ItemData>($"Data/Items/{name}Data");
    }

    public void SetName(string name)
    {
        this.name = name;
    }
}
