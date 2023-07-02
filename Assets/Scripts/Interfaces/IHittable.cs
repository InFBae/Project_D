using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHittable 
{
    public enum HitType { Weak, Strong }
    public void TakeHit(float damage, GameObject attacker, HitType hitType);

    public void Die();
}
