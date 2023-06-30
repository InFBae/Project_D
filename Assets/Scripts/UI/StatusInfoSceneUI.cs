using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StatusInfoSceneUI : SceneUI
{
    protected override void Awake()
    {
        base.Awake();
        SetEXP(GameManager.Data.CurEXP);
    }

    private void OnEnable()
    {
        GameManager.Data.OnEXPChanged += SetEXP;
    }
    private void OnDisable()
    {
        GameManager.Data.OnEXPChanged -= SetEXP;
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
    public void SetEXP(int count)
    {
        texts["EXPCount"].text = count.ToString();
    }

}
