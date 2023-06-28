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
            // TODO �� ���� ���ϱ�;
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
    
    public int vitality;  // hp ���
    public int endurance; // sp ���
    public int resistance;    // dp ���
    public int strength;    // ���ݷ�, dp ���
    public int dexerity;     // ���ݷ� ���

    // ���� ��
    public string savedScene;      
    public Transform savedSpawnPoint;

    // �� ������
    public List<ItemData> quickItemList;
    public int quickItemIndex;

    // �κ��丮
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
