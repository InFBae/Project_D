using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatusData", menuName = "Data/PlayerStatusData")]
public class PlayerStatusData : ScriptableObject
{
    public float maxHP;
    public float maxSP;
    public float spRechargeTime;
    public float DP;
    public WeaponData weaponData;
}
