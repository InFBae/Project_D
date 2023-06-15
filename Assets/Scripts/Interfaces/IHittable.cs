using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHittable 
{
    public void TakeHit(float damage);

    public void Die();
}
