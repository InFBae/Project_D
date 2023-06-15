using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class Monster : MonoBehaviour, IHittable
{  
    protected Animator animator;
    protected Collider coll;
    protected Renderer rend;
    protected Rigidbody rb;

    protected MonsterData monsterData;
    protected float curHP;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        coll = GetComponent<Collider>();
        rend = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
    }
    public virtual void TakeHit(float damage)
    {
        curHP -= damage;
    }

    public virtual void Die()
    {
        GameManager.Resource.Destroy(gameObject);
    }
}


