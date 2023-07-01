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
    protected Dictionary<IHittable, float> hitTable;

    protected GameObject target;
    private float curHP;

    public UnityEvent<float> OnHPChanged;
    public float MaxHP { get { return monsterData.maxHP; } }
    public float CurHP { get { return curHP; } set { curHP = value; OnHPChanged?.Invoke(CurHP); } }
    public GameObject Target { get { return target; } }
    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        coll = GetComponent<Collider>();
        rend = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();

        hitTable = new Dictionary<IHittable, float>();
    }
    public virtual void DropItem()
    {

    }
    public virtual void TakeHit(float damage, GameObject attacker)
    {
        CurHP -= damage;
    }

    public virtual void Die()
    {
        GameManager.Pool.Release(gameObject);
    }
}


