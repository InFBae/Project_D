using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Data/Weapon")]
public class WeaponData : ScriptableObject
{
    string resourceRoot;
    public float damage;
    public float attackCooltime;
    public Transform offset;
}
