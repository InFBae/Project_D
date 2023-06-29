using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, ICollectable
{
    protected ItemData data;
    [SerializeField] protected int count;
    public ItemData Data { get { return data; } }
    public int Count { get { return count; } }
    public virtual void Collect()
    {
        GameManager.Data.PlayerStatusData.inventory.Add(this);
        GameManager.Resource.Destroy(this.gameObject);
    }

    public void IncreaseCount(int num)
    {
        count += num;
    }

    public void DecreaseCount(int num)
    {
        count -= num;
    }
}
