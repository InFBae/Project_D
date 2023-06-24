using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHittable 
{
    public void TakeHit(float damage, GameObject attacker);

    public void Die();
}
