using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

public abstract class Monster : MonoBehaviour, IHittable
{  
    protected Animator animator;
    protected Collider coll;
    protected Renderer rend;
    protected Rigidbody rb;

    protected MonsterData monsterData;

    private float curHP;

    public UnityEvent<float> OnHPChanged;
    public float MaxHP { get { return monsterData.maxHP; } }
    public float CurHP { get { return curHP; } set { curHP = value; OnHPChanged?.Invoke(CurHP); } }
    
    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        coll = GetComponent<Collider>();
        rend = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
    }
    public virtual void TakeHit(float damage)
    {
        CurHP -= damage;
    }

    public virtual void Die()
    {
        GameManager.Resource.Destroy(gameObject);
    }
}


