using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerStatusController : MonoBehaviour
{
    [SerializeField] private StatusInfoSceneUI statusInfoSceneUI;
    private PlayerStatusData statusData;

    public static UnityAction OnStatusChanged; 

    private void Awake()
    {
        statusData = GameManager.Resource.Load<PlayerStatusData>("Data/PlayerStatusData");
    }

    private void Start()
    {
        StatusUpdate();
        curHP = maxHP;
        curSP = maxSP;
        spRechargeTime = statusData.spRechargeTime;

        statusInfoSceneUI.SetLeftWeapon(statusData.leftWeapon.sprite);
        statusInfoSceneUI.SetRightWeapon(statusData.rightWeapon.sprite);
        statusInfoSceneUI.SetQuickSlot();
    }

    private void OnEnable()
    {
        OnStatusChanged += StatusUpdate;
    }

    private void OnDisable()
    {
        OnStatusChanged -= StatusUpdate;
    }

    private void Update()
    {
        SPRechargeTime();
        SPRecover();
        GaugeUpdate();
    }

    private void GaugeUpdate()
    {
        statusInfoSceneUI.SetHP(curHP/maxHP);
        statusInfoSceneUI.SetSP(curSP/maxSP);
    }

    private void StatusUpdate()
    {
        maxHP = statusData.maxHP + statusData.vitality * 10;
        maxSP = statusData.maxSP + statusData.endurance * 10;
        curDP = statusData.DP;
    }

    #region HP
    private float maxHP;
    private float curHP;

    public void IncreaseHP(float hp)
    {
        if(curHP + hp < maxHP) { curHP += hp; }
        else { curHP = maxHP; }
    }

    public void DecreaseHP(float hp)
    {
        float damage = hp;

        if (curDP > 0) 
        {
            damage = (hp - curDP) > 0 ? hp - curDP : 0;
        }
        curHP -= damage;

        if(curHP <= 0)
        {
            PlayerStateController.OnPlayerDied?.Invoke();
        }
    }

    #endregion

    #region SP
    private float maxSP;
    private float curSP;

    // 스태미나 증가량
    private float spIncreaseSpeed = 10;

    // 스태미나 재회복 딜레이 시간
    private float spRechargeTime;
    private float currentSpRechargeTime;

    // 스태미나 감소 여부
    private bool spUsed;

    public void DecreaseSP(float sp)
    {
        spUsed = true;
        currentSpRechargeTime = 0;

        if (curSP - sp > 0)
        {
            curSP -= sp;
        }
        else
            curSP = 0;
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
        if (!spUsed && curSP < maxSP)
        {
            if(curSP + spIncreaseSpeed * Time.deltaTime > maxSP)
            {
                curSP = maxSP;
            }
            else curSP += spIncreaseSpeed * Time.deltaTime;
        }
    }

    public float GetCurrentSP()
    {
        return curSP;
    }

    #endregion

    #region DP
    private float curDP;

    public void IncreaseDP(float dp)
    {
        curDP += dp;
    }

    public void DecreaseDP(float dp)
    {
        if(curDP- dp > 0)
        {
            curDP -= dp;
        }
        else
        {
            curDP = 0;
        }
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
            (GameManager.Data.PlayerStatusData.quickItemList[GameManager.Data.PlayerStatusData.quickItemIndex] as IUsable).Use();
            StatusInfoSceneUI.OnQuickSlotChanged?.Invoke();
        }       
    }

    public void DIsableStatusSceneUI()
    {
        statusInfoSceneUI.enabled = false;
    }

}
