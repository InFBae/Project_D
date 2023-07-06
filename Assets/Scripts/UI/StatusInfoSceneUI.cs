using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class StatusInfoSceneUI : SceneUI
{
    public static UnityAction OnQuickSlotChanged;
    protected override void Awake()
    {
        base.Awake();
        SetHP(GameManager.Data.CurHP);
        SetSP(GameManager.Data.CurSP);
        SetEXP(GameManager.Data.CurEXP);
        SetQuickSlot();
    }

    private void OnEnable()
    {
        GameManager.Data.OnEXPChanged += SetEXP;
        GameManager.Data.OnHPChanged += SetHP;
        GameManager.Data.OnSPChanged += SetSP;
        OnQuickSlotChanged += SetQuickSlot;
    }
    private void OnDisable()
    {
        GameManager.Data.OnEXPChanged -= SetEXP;
        GameManager.Data.OnHPChanged -= SetHP;
        GameManager.Data.OnSPChanged -= SetSP;
        OnQuickSlotChanged -= SetQuickSlot;
    }

    public void SetHP(float HP)
    {
        sliders["HPBar"].value = HP / GameManager.Data.PlayerStatusData.MaxHP;
    }

    public void SetSP(float SP)
    {
        sliders["SPBar"].value = SP / GameManager.Data.PlayerStatusData.MaxSP;
    }

    public void SetLeftWeapon(Sprite sprite)
    {
        images["LeftWeaponImage"].sprite = sprite;
    }    

    public void SetRightWeapon(Sprite sprite)
    {
        images["RightWeaponImage"].sprite = sprite;
    }

    public void SetQuickSlot()
    {
        if (GameManager.Data.PlayerStatusData.quickItemList.Count > 0)
        {
            int quickItemIndex = GameManager.Data.PlayerStatusData.quickItemIndex % GameManager.Data.PlayerStatusData.quickItemList.Count;
            images["QuickItemImage"].color = Color.white;
            images["QuickItemImage"].sprite = GameManager.Data.PlayerStatusData.quickItemList[quickItemIndex].Data.sprite;
            if (GameManager.Data.PlayerStatusData.quickItemList[quickItemIndex].Count > 0)
            {
                texts["QuickItemCount"].enabled = true;
                texts["QuickItemCount"].text = GameManager.Data.PlayerStatusData.quickItemList[quickItemIndex].Count.ToString();
            }
            else
            {
                GameManager.Data.PlayerStatusData.quickItemList.RemoveAt(quickItemIndex);
                /*images["QuickItemImage"].color = Color.black;
                images["QuickItemImage"].sprite = null;
                texts["QuickItemCount"].enabled = false;*/
                SetQuickSlot();
            }
        }
        else
        {
            images["QuickItemImage"].color = Color.black;
            images["QuickItemImage"].sprite = null;
            texts["QuickItemCount"].enabled = false;
        }
        
        if (GameManager.Data.PlayerStatusData.quickItemList.Count > 1)
        {
            int nextItemIndex = (GameManager.Data.PlayerStatusData.quickItemIndex + 1) % GameManager.Data.PlayerStatusData.quickItemList.Count;
            images["NextItemImage"].color = Color.white;
            images["NextItemImage"].sprite = GameManager.Data.PlayerStatusData.quickItemList[nextItemIndex].Data.sprite; 
        }
        else
        {
            images["NextItemImage"].color = Color.black;
            images["NextItemImage"].sprite = null;
        }
    }

    /*
    public void SetQuickItem(Sprite sprite)
    {      
        if (sprite == null)
            images["QuickItemImage"].color = Color.black;
        else
            images["QuickItemImage"].color = Color.white;

        images["QuickItemImage"].sprite = sprite;
    }
    public void SetNextItem(Sprite sprite)
    {
        if (sprite == null)
            images["NextItemImage"].color = Color.black;
        else
            images["NextItemImage"].color = Color.white;
        images["NextItemImage"].sprite = sprite;
    }
    public void SetQuickItemCount(int count)
    {
        if (count > 0)
        {
            texts["QuickItemCount"].enabled = true;
            texts["QuickItemCount"].text = count.ToString();
            images["QuickItemImage"].color = Color.white;
        }
        else
        {
            texts["QuickItemCount"].enabled = false;
            texts["QuickItemCount"].text = count.ToString();
            images["QuickItemImage"].color = Color.black;
        }       
    }
    */
    public void SetEXP(int count)
    {
        texts["EXPCount"].text = count.ToString();
    }

}
