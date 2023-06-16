using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    protected WeaponData weaponData;
    protected string weaponName;

    protected virtual void Awake()
    {
        weaponData = GameManager.Resource.Load<WeaponData>($"Data/Weapons/{weaponName}");        
    }
}
