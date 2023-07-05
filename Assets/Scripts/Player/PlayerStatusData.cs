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
            // TODO �� ���� ���ϱ�;
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
    
    public int vitality;  // hp ���
    public int endurance; // sp ���
    public int resistance;    // dp ���
    public int strength;    // ���ݷ�, dp ���
    public int dexerity;     // ���ݷ� ���

    // ���� ��
    public string savedScene;      
    public int savedSpawnPointIndex;

    // �κ��丮
    public List<Item> inventory;

    // �� ������
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
