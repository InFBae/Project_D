using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OHSword : Weapon
{
    protected override void Awake()
    {
        weaponName = "1HSword";
        base.Awake();
    }

    private void OnTriggerEnter(Collider other)
    {
        IHittable hittable = other.GetComponent<IHittable>();
        hittable?.TakeHit(weaponData.damage);
    }
}
