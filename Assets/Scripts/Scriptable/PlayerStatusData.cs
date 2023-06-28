using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatusData", menuName = "Data/PlayerStatusData")]
public class PlayerStatusData : ScriptableObject, ISerializationCallbackReceiver
{
    public float maxHP;
    public float maxSP;
    public float spRechargeTime;
    public float AP { 
        get 
        {
            float damage = 0;
            damage += rightWeapon.damage;
            if ((rightWeapon.weaponType & WeaponData.WeaponType.Strength) > 0)
            {
                damage += strength;
            }
            if (((rightWeapon.weaponType & WeaponData.WeaponType.Agility) > 0))
            {
                damage += (dexerity * 1.5f);
            }
            return damage; 
        } } 
    public float DP
    {
        get
        {
            float defense = 0;
            // TODO 방어구 방어력 더하기;
            defense += (vitality * 1.5f);
            defense += (strength * 0.5f);
            return defense;
        }
    }
    public WeaponData leftWeapon;
    public WeaponData rightWeapon;

    public ArmorData armorHead;
    public ArmorData armorHand;
    public ArmorData armorUpperBody;
    public ArmorData armorLowerBody;    
    
    public int vitality;  // hp 상승
    public int endurance; // sp 상승
    public int resistance;    // dp 상승
    public int strength;    // 공격력, dp 상승
    public int dexerity;     // 공격력 상승

    // 현재 맵
    public string savedScene;      
    public Transform savedSpawnPoint;

    // 퀵 아이템
    public List<ItemData> quickItemList;
    public int quickItemIndex;

    // 인벤토리
    public List<ItemData> inventory;

    public int EXP;

    public void OnBeforeSerialize()
    {
        if (inventory == null)
        {
            inventory = new List<ItemData>();
        }
    }

    public void OnAfterDeserialize()
    {
        
    }
}
