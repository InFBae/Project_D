using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    protected WeaponData weaponData;
    protected string weaponName;
    protected Collider coll;

    protected virtual void Awake()
    {
        weaponData = GameManager.Resource.Load<WeaponData>($"Data/Weapons/{weaponName}");
        coll = GetComponent<Collider>();
        if( coll != null )
            coll.enabled = false;
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
