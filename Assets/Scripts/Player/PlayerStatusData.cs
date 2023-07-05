using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerStatusData : ISerializationCallbackReceiver
{
    public float defaultHP;
    public float defaultSP;
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
    public float MaxHP { get { return defaultHP + vitality * 10; } }
    public float MaxSP { get { return defaultSP + endurance * 10; } }

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
    public int savedSpawnPointIndex;

    // 인벤토리
    public List<Item> inventory;

    // 퀵 아이템
    public List<Item> quickItemList;
    public int quickItemIndex;

    public int EXP;

    public void OnBeforeSerialize()
    {
        if (inventory == null)
        {
            inventory = new List<Item>();
        }
        if (quickItemList == null)
        {
            quickItemList = new List<Item>();
            quickItemIndex = 0;
        }
    }

    public void OnAfterDeserialize()
    {
        
    }
}
