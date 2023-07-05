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
    public PlayerStateController.State PlayerState { get { return playerState; } set { playerState = value; } }

    public UnityAction<int> OnEXPChanged;

    private void Start()
    {
        playerStatusData = new PlayerStatusData();
        path = Path.Combine(Application.dataPath, "saveData.json");
        LoadData();
    }

    public void SaveData()
    {
        PlayerStatusData playerSavedData = new PlayerStatusData();
        playerSavedData.maxHP = playerStatusData.maxHP;
        playerSavedData.maxSP = playerStatusData.maxSP;
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

        if (playerSavedData.quickItemList == null)
            playerSavedData.quickItemList = new List<Item>();
        if (playerStatusData.quickItemList != null)
        {
            playerSavedData.quickItemList.Clear();
            foreach (Item item in playerStatusData.quickItemList)
            {
                playerSavedData.quickItemList.Add(item);
            }
            playerSavedData.quickItemIndex = playerStatusData.quickItemIndex;
        }
        
        if (playerSavedData.inventory == null)
            playerSavedData.inventory = new List<Item>();
        if (playerStatusData.inventory != null)
        {
            playerSavedData.inventory.Clear();
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
                playerStatusData.maxHP = playerSavedData.maxHP;
                playerStatusData.maxSP = playerSavedData.maxSP;
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

                if (playerStatusData.quickItemList == null)
                    playerStatusData.quickItemList = new List<Item>();
                if (playerSavedData.quickItemList != null)
                {
                    playerStatusData.quickItemList.Clear();
                    foreach (Item item in playerSavedData.quickItemList)
                    {
                        playerStatusData.quickItemList.Add(item);
                    }
                    playerStatusData.quickItemIndex = playerSavedData.quickItemIndex;
                }

                if (playerStatusData.inventory == null)
                    playerStatusData.inventory = new List<Item>();
                if (playerSavedData.inventory != null)
                {
                    playerStatusData.inventory.Clear();
                    foreach (Item item in playerSavedData.inventory)
                    {
                        playerStatusData.inventory.Add(item);
                    }
                }

                playerStatusData.EXP = playerSavedData.EXP;
                CurEXP = playerSavedData.EXP;
            }
        } 
    }

    public void ClearData()
    {
        playerStatusData.maxHP = 100;
        playerStatusData.maxSP = 100;
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

        SaveData();
    }
}