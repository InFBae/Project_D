using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DataManager : MonoBehaviour
{
    private PlayerStatusData playerStatusData;
    private PlayerStatusData playerSavedData;
    private Transform playerTransform;
    public PlayerStatusData PlayerStatusData { get { return playerStatusData; } }
    public Transform PlayerTransform { 
        get { return playerTransform; } 
        set { playerTransform = value; }
    }


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
    }
}