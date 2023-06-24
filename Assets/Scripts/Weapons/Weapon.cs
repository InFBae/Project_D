using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    protected WeaponData weaponData;
    protected string weaponName;
    protected Collider coll;
    public Dictionary<IHittable, float> hitTable;

    public GameObject owner;

    protected virtual void Awake()
    {
        weaponData = GameManager.Resource.Load<WeaponData>($"Data/Weapons/{weaponName}");
        coll = GetComponent<Collider>();
        if( coll != null )
            coll.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        IHittable hittable = other.GetComponent<IHittable>();
        if (hittable != null)
        {
            if (hitTable.TryAdd(hittable, GetDamage()))
            {
                hittable.TakeHit(GetDamage(), owner);
            }
        }
    }

    public float GetDamage()
    {
        return weaponData.damage;
    }

    public float GetAttackCoolTime()
    {
        return weaponData.attackCooltime;
    }

    public Transform GetOffset() 
    {
        return weaponData.offset;
    }

    public void EnableCollider()
    {
        if ( coll != null )
        {
            coll.enabled = true;
        }
    }

    public void DisableCollider()
    {
        if ( coll != null )
        {
            coll.enabled = false;
        }
    }
}
