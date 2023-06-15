using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Data/Monster")]
public class MonsterData : ScriptableObject
{
    public float maxHP;
    public float damage;
    public float speed;
    public float attackCooltime;
    public float detectRange;
    public float attackRange;
}
