using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class DataManager : MonoBehaviour
{
    private PlayerStatusData playerStatusData;
    private Transform playerTransform;
    string path;

    private int curEXP;
    private float curHP;
    private float curSP;

    private PlayerStateController.State playerState;
    public PlayerStatusData PlayerStatusData { get { return playerStatusData; } }
    public Transform PlayerTransform { 
        get { return playerTransform; } 
        set { playerTransform = value; }
    }
    public int CurEXP { get { return curEXP; } set {  curEXP = value;  OnEXPChanged?.Invoke(curEXP); } }
    public float CurHP { get { return curHP; } set { curHP = value; OnHPChanged?.Invoke(curHP); } }
    public float CurSP { get { return curSP; } set { curSP = value; OnSPChanged?.Invoke(curSP); } }
    public PlayerStateController.State PlayerState { get { return playerState; } set { playerState = value; } }

    public UnityAction<int> OnEXPChanged;
    public UnityAction<float> OnHPChanged;
    public UnityAction<float> OnSPChanged;

    private void Start()
    {
        playerStatusData = new PlayerStatusData();
        path = Path.Combine(Application.dataPath, "saveData.json");
        LoadData();
    }

    public void IncreaseHP(float hp)
    {
        if (GameManager.Data.CurHP + hp < GameManager.Data.PlayerStatusData.MaxHP) { GameManager.Data.CurHP += hp; }
        else { GameManager.Data.CurHP = GameManager.Data.PlayerStatusData.MaxHP; }
    }

    public void DecreaseHP(float hp)
    {
        float damage = hp;

        if (PlayerStatusData.DP > 0)
        {
            damage = (hp - PlayerStatusData.DP) > 0 ? hp - PlayerStatusData.DP : 0;
        }
        GameManager.Data.CurHP -= damage;

        if (GameManager.Data.CurHP <= 0)
        {
            PlayerStateController.OnPlayerDied?.Invoke();
        }
    }


    public void SaveData()
    {
        PlayerStatusData playerSavedData = new PlayerStatusData();
        playerSavedData.defaultHP = playerStatusData.defaultHP;
        playerSavedData.defaultSP = playerStatusData.defaultSP;
        playerSavedData.spRechargeTime = playerStatusData.spRechargeTime;
        
        playerSavedData.leftWeapon = playerStatusData.leftWeapon;
        playerSavedData.rightWeapon = playerStatusData.rightWeapon;

        playerSavedData.vitality = playerStatusData.vitality;
        playerSavedData.endurance = playerStatusData.endurance;
        playerSavedData.resistance = playerStatusData.resistance;
        playerSavedData.strength = playerStatusData.strength;
        playerSavedData.dexerity = playerStatusData.dexerity;        
    
        playerSavedData.savedScene = playerStatusData.savedScene;
        playerSavedData.savedSpawnPointIndex = playerStatusData.savedSpawnPointIndex;

        playerSavedData.quickItemList = new List<Item>();
        if (playerStatusData.quickItemList != null)
        {
            foreach (Item item in playerStatusData.quickItemList)
            {
                playerSavedData.quickItemList.Add(item);
            }
            playerSavedData.quickItemIndex = playerStatusData.quickItemIndex;
        }
        
        playerSavedData.inventory = new List<Item>();
        if (playerStatusData.inventory != null)
        {
            foreach (Item item in playerStatusData.inventory)
            {
                playerSavedData.inventory.Add(item);
            }
        }
        
        playerSavedData.EXP = CurEXP;

        string json = JsonUtility.ToJson(playerSavedData, true);

        File.WriteAllText(path, json);
    }
    public void LoadData()
    {
        PlayerStatusData playerSavedData = new PlayerStatusData();

        if (!File.Exists(path))
        {
            ClearData();
        }
        else
        {
            string loadJson = File.ReadAllText(path);
            playerSavedData = JsonUtility.FromJson<PlayerStatusData>(loadJson);

            if (playerSavedData != null)
            {
                playerStatusData.defaultHP = playerSavedData.defaultHP;
                playerStatusData.defaultSP = playerSavedData.defaultSP;
                playerStatusData.spRechargeTime = playerSavedData.spRechargeTime;

                playerStatusData.leftWeapon = playerSavedData.leftWeapon;
                playerStatusData.rightWeapon = playerSavedData.rightWeapon;

                playerStatusData.vitality = playerSavedData.vitality;
                playerStatusData.endurance = playerSavedData.endurance;
                playerStatusData.resistance = playerSavedData.resistance;
                playerStatusData.strength = playerSavedData.strength;
                playerStatusData.dexerity = playerSavedData.dexerity;

                playerStatusData.savedScene = playerSavedData.savedScene;
                playerStatusData.savedSpawnPointIndex = playerSavedData.savedSpawnPointIndex;

                playerStatusData.inventory = new List<Item>();
                if (playerSavedData.inventory != null)
                {
                    foreach (Item item in playerSavedData.inventory)
                    {
                        if (item.name == "RedPotion")
                        {
                            RedPotion redPotion = new RedPotion();
                            redPotion.SetCount(item.count);
                            playerStatusData.inventory.Add(redPotion);
                        }
                        else if (item.name == "BluePotion")
                        {
                            BluePotion bluePotion = new BluePotion();
                            bluePotion.SetCount(item.count);
                            playerStatusData.inventory.Add(bluePotion);
                        }
                    }
                }

                playerStatusData.quickItemList = new List<Item>();
                if (playerSavedData.quickItemList != null)
                {
                    foreach (Item item in playerSavedData.quickItemList)
                    {
                        foreach(Item inventoryItem in GameManager.Data.PlayerStatusData.inventory)
                        {
                            if(item.name == inventoryItem.name)
                            {
                                playerStatusData.quickItemList.Add(inventoryItem);
                                break;
                            }
                        }                        
                    }
                    playerStatusData.quickItemIndex = playerSavedData.quickItemIndex;
                }

                playerStatusData.EXP = playerSavedData.EXP;
                CurEXP = playerSavedData.EXP;
                CurHP = playerStatusData.MaxHP;
                CurSP = playerStatusData.MaxSP;
            }
        } 
    }

    public void ClearData()
    {
        playerStatusData.defaultHP = 100;
        playerStatusData.defaultSP = 100;
        playerStatusData.spRechargeTime = 1.5f;

        playerStatusData.leftWeapon = GameManager.Resource.Load<WeaponData>("Data/Weapons/Shield");
        playerStatusData.rightWeapon = GameManager.Resource.Load<WeaponData>("Data/Weapons/OHSword");

        playerStatusData.vitality = 1;
        playerStatusData.endurance = 1;
        playerStatusData.resistance = 1;
        playerStatusData.strength = 1;
        playerStatusData.dexerity = 1;

        playerStatusData.savedScene = null;
        playerStatusData.savedSpawnPointIndex = 0;

        playerStatusData.quickItemList = new List<Item>();
        playerStatusData.inventory = new List<Item>();

        playerStatusData.EXP = 0;
        CurEXP = 0;

        SaveData();
    }
}