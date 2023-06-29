using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStatusController : MonoBehaviour
{
    [SerializeField] private StatusInfoSceneUI statusInfoSceneUI;
    private PlayerStatusData statusData;

    private const int HP = 0, SP = 1, DP = 2;

    private void Awake()
    {
        statusData = GameManager.Resource.Load<PlayerStatusData>("Data/PlayerStatusData");
    }

    private void Start()
    {
        maxHP = statusData.maxHP;
        curHP = maxHP;
        maxSP = statusData.maxSP;
        curSP = maxSP;
        spRechargeTime = statusData.spRechargeTime;
        curDP = statusData.DP;

        statusInfoSceneUI.SetLeftWeapon(statusData.leftWeapon.sprite);
        statusInfoSceneUI.SetRightWeapon(statusData.rightWeapon.sprite);
        if(statusData.quickItemList.Count > 0)
        {
            statusInfoSceneUI.SetQuickItem(statusData.quickItemList[statusData.quickItemIndex].Data.sprite);
            statusInfoSceneUI.SetQuickItemCount(statusData.quickItemList[statusData.quickItemIndex].Count);
        }
        else
        {
            statusInfoSceneUI.SetQuickItem(null);
            statusInfoSceneUI.SetQuickItemCount(0);
        }
        if(statusData.quickItemList.Count > 1)
        {
            statusInfoSceneUI.SetNextItem(statusData.quickItemList[(statusData.quickItemIndex + 1) % statusData.quickItemList.Count].Data.sprite);
        }
        else
        {
            statusInfoSceneUI.SetNextItem(null);
        }
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
            // TODO: GameOver;
        }
    }

    #endregion

    #region SP
    private float maxSP;
    private float curSP;

    // 스태미나 증가량
    private float spIncreaseSpeed = 2;

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

        statusInfoSceneUI.SetQuickItem(statusData.quickItemList[statusData.quickItemIndex].Data.sprite);
        statusInfoSceneUI.SetQuickItemCount(statusData.quickItemList[statusData.quickItemIndex].Count);

        if (statusData.quickItemList.Count > 1)
        {
            statusInfoSceneUI.SetNextItem(statusData.quickItemList[(statusData.quickItemIndex + 1) % statusData.quickItemList.Count].Data.sprite);
        }
    }

}
