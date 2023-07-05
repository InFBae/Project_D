using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item
{
    protected ItemData data;
    [SerializeField] protected int count;
    public PlayerStatusController owner;

    public ItemData Data { get { return data; } }
    public int Count { get { return count; } }

    public void SetCount(int num)
    {
        count = num;
    }
}
