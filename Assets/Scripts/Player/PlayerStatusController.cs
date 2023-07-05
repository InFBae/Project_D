using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerStatusController : MonoBehaviour
{
    [SerializeField] private StatusInfoSceneUI statusInfoSceneUI;
    private PlayerStatusData statusData;

    private void Awake()
    {
        statusData = GameManager.Data.PlayerStatusData;
    }

    private void Start()
    {
        spRechargeTime = statusData.spRechargeTime;

        statusInfoSceneUI.SetLeftWeapon(statusData.leftWeapon.sprite);
        statusInfoSceneUI.SetRightWeapon(statusData.rightWeapon.sprite);
        statusInfoSceneUI.SetQuickSlot();
    }

    private void Update()
    {
        SPRechargeTime();
        SPRecover();
    }


    #region HP

    public void IncreaseHP(float hp)
    {
        if(GameManager.Data.CurHP + hp < GameManager.Data.PlayerStatusData.MaxHP) { GameManager.Data.CurHP += hp; }
        else { GameManager.Data.CurHP = GameManager.Data.PlayerStatusData.MaxHP; }
    }

    public void DecreaseHP(float hp)
    {
        float damage = hp;

        if (GameManager.Data.PlayerStatusData.DP > 0) 
        {
            damage = (hp - GameManager.Data.PlayerStatusData.DP) > 0 ? hp - GameManager.Data.PlayerStatusData.DP : 0;
        }
        GameManager.Data.CurHP -= damage;

        if(GameManager.Data.CurHP <= 0)
        {
            PlayerStateController.OnPlayerDied?.Invoke();
        }
    }

    #endregion

    #region SP

    // 스태미나 증가량
    private float spIncreaseSpeed = 20;

    // 스태미나 재회복 딜레이 시간
    private float spRechargeTime;
    private float currentSpRechargeTime;

    // 스태미나 감소 여부
    private bool spUsed;

    public void DecreaseSP(float sp)
    {
        spUsed = true;
        currentSpRechargeTime = 0;

        if (GameManager.Data.CurSP - sp > 0)
        {
            GameManager.Data.CurSP -= sp;
        }
        else
            GameManager.Data.CurSP = 0;
    }

    private void SPRechargeTime()
    {
        if (spUsed)
        {
            if (currentSpRechargeTime < spRechargeTime)
                currentSpRechargeTime += Time.deltaTime;
            else
                spUsed = false;
        }
    }

    private void SPRecover()
    {
        if (!spUsed && GameManager.Data.CurSP < GameManager.Data.PlayerStatusData.MaxSP)
        {
            if(GameManager.Data.CurSP + spIncreaseSpeed * Time.deltaTime > GameManager.Data.PlayerStatusData.MaxSP)
            {
                GameManager.Data.CurSP = GameManager.Data.PlayerStatusData.MaxSP;
            }
            else GameManager.Data.CurSP += spIncreaseSpeed * Time.deltaTime;
        }
    }

    public float GetCurrentSP()
    {
        return GameManager.Data.CurSP;
    }

    #endregion

    private void OnChangeItem(InputValue input)
    {
        if (statusData.quickItemList.Count < 1)
            return;
        if(input.Get<float>() > 0.5)
        {
            statusData.quickItemIndex = (statusData.quickItemIndex + 1) % statusData.quickItemList.Count;            
        }
        else if(input.Get<float>() < -0.5)
        {
            statusData.quickItemIndex = statusData.quickItemIndex - 1 < 0 ? statusData.quickItemList.Count - 1 : statusData.quickItemIndex - 1;            
        }

        statusInfoSceneUI.SetQuickSlot();
    }

    private void OnUseItem()
    {
        if (GameManager.Data.PlayerStatusData.quickItemList.Count > 0)
        {
            int quickItemIndex = GameManager.Data.PlayerStatusData.quickItemIndex;
            /*if (GameManager.Data.PlayerStatusData.inventory[quickItemIndex].Count == 1)
            {
                (GameManager.Data.PlayerStatusData.inventory[quickItemIndex] as IUsable).Use();

            }*/
            (GameManager.Data.PlayerStatusData.quickItemList[quickItemIndex] as IUsable).Use();
            StatusInfoSceneUI.OnQuickSlotChanged?.Invoke();
        }       
    }

    public void DIsableStatusSceneUI()
    {
        statusInfoSceneUI.enabled = false;
    }

}
