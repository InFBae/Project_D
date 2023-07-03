using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DataManager : MonoBehaviour
{
    private PlayerStatusData playerStatusData;
    private PlayerStatusData playerSavedData;
    private Transform playerTransform;
    private int curEXP;
    private PlayerStateController.State playerState;
    public PlayerStatusData PlayerStatusData { get { return playerStatusData; } }
    public Transform PlayerTransform { 
        get { return playerTransform; } 
        set { playerTransform = value; }
    }
    public int CurEXP { get { return curEXP; } set {  curEXP = value;  OnEXPChanged?.Invoke(curEXP); } }
    public PlayerStateController.State PlayerState { get { return playerState; } set { playerState = value; } }

    public UnityAction<int> OnEXPChanged;

    private void Awake()
    {
        playerStatusData = GameManager.Resource.Load<PlayerStatusData>("Data/PlayerStatusData");
        playerSavedData = GameManager.Resource.Load<PlayerStatusData>("Data/PlayerSavedData");
        LoadData();
    }

    public void SaveData()
    {
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
        playerSavedData.savedSpawnPoint = playerStatusData.savedSpawnPoint;

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
        

        playerSavedData.EXP = curEXP;
    }
    private void LoadData()
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
        playerStatusData.savedSpawnPoint = playerSavedData.savedSpawnPoint;

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