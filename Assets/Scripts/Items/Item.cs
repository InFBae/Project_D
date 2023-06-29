using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    protected ItemData data;
    [SerializeField] protected int count;
    public PlayerStatusController owner;

    public ItemData Data { get { return data; } }
    public int Count { get { return count; } }

    public void IncreaseCount(int num)
    {
        count += num;
    }

    public void DecreaseCount(int num)
    {
        count -= num;
    }
}
