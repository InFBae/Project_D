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
        SetEXP(GameManager.Data.CurEXP);
        SetQuickSlot();
    }

    private void OnEnable()
    {
        GameManager.Data.OnEXPChanged += SetEXP;
        OnQuickSlotChanged += SetQuickSlot;
    }
    private void OnDisable()
    {
        GameManager.Data.OnEXPChanged -= SetEXP;
        OnQuickSlotChanged -= SetQuickSlot;
    }

    public void SetHP(float HP)
    {
        sliders["HPBar"].value = HP;
    }

    public void SetSP(float SP)
    {
        sliders["SPBar"].value = SP;
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
        List<Item> quickItemList = GameManager.Data.PlayerStatusData.quickItemList;
        int index = GameManager.Data.PlayerStatusData.quickItemIndex;
        if (quickItemList.Count > 0)
        {
            images["QuickItemImage"].color = Color.white;
            images["QuickItemImage"].sprite = quickItemList[index].Data.sprite;
            if (quickItemList[index].Count > 0)
            {
                texts["QuickItemCount"].enabled = true;
                texts["QuickItemCount"].text = quickItemList[index].Count.ToString();
            }
            else
            {
                quickItemList.RemoveAt(index);         
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
        
        if (quickItemList.Count > 1)
        {
            images["NextItemImage"].color = Color.white;
            images["NextItemImage"].sprite = quickItemList[(index + 1) % quickItemList.Count].Data.sprite; ;
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
