using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentUI : BaseUI
{
    PlayerStatusData statusData;
    protected override void Awake()
    {
        base.Awake();
        statusData = GameManager.Data.PlayerStatusData;
    }

    private void OnEnable()
    {
        InitUI();
    }
    public void InitUI()
    {
        // armorHead
        if(statusData.armorHead != null)
        {
            images["EquipmentHeadIcon"].color = Color.white;
            images["EquipmentHeadIcon"].sprite = statusData.armorHead.sprite;
            texts["EquipmentHeadText"].text = statusData.armorHead.instruction;
        }
        else
        {
            images["EquipmentHeadIcon"].color = Color.black;
            texts["EquipmentHeadText"].text = "None";
        }

        // armorHand
        if (statusData.armorHand != null)
        {
            images["EquipmentHandIcon"].color = Color.white;
            images["EquipmentHandIcon"].sprite = statusData.armorHand.sprite;
            texts["EquipmentHandText"].text = statusData.armorHand.instruction;
        }
        else
        {
            images["EquipmentHandIcon"].color = Color.black;
            texts["EquipmentHandText"].text = "None";
        }

        // armorUpperBody
        if (statusData.armorUpperBody != null)
        {
            images["EquipmentUpperBodyIcon"].color = Color.white;
            images["EquipmentUpperBodyIcon"].sprite = statusData.armorUpperBody.sprite;
            texts["EquipmentUpperBodyText"].text = statusData.armorUpperBody.instruction;
        }
        else
        {
            images["EquipmentUpperBodyIcon"].color = Color.black;
            texts["EquipmentUpperBodyText"].text = "None";
        }

        // armorLowerBody
        if (statusData.armorLowerBody != null)
        {
            images["EquipmentLowerBodyIcon"].color = Color.white;
            images["EquipmentLowerBodyIcon"].sprite = statusData.armorLowerBody.sprite;
            texts["EquipmentLowerBodyText"].text = statusData.armorLowerBody.instruction;
        }
        else
        {
            images["EquipmentLowerBodyIcon"].color = Color.black;
            texts["EquipmentLowerBodyText"].text = "None";
        }

        // weaponLeft
        if (statusData.leftWeapon != null)
        {
            images["WeaponLeftIcon"].color = Color.white;
            images["WeaponLeftIcon"].sprite = statusData.leftWeapon.sprite;
            texts["WeaponLeftText"].text = statusData.leftWeapon.instruction;
        }
        else
        {
            images["WeaponLeftIcon"].color = Color.black;
            texts["WeaponLeftText"].text = "None";
        }

        // weaponRight
        if (statusData.rightWeapon != null)
        {
            images["WeaponRightIcon"].color = Color.white;
            images["WeaponRightIcon"].sprite = statusData.rightWeapon.sprite;
            texts["WeaponRightText"].text = statusData.rightWeapon.instruction;
        }
        else
        {
            images["WeaponRightIcon"].color = Color.black;
            texts["WeaponRightText"].text = "None";
        }

        // quickItem
        if (statusData.quickItemList.Count > 0)
        {
            images["QuickItemIcon"].color = Color.white;
            images["QuickItemIcon"].sprite = statusData.quickItemList[statusData.quickItemIndex].Data.sprite;
            texts["QuickItemText"].text = statusData.quickItemList[statusData.quickItemIndex].Data.instruction;
        }
        else
        {
            images["QuickItemIcon"].color = Color.black;
            images["QuickItemIcon"].sprite = null;
            texts["QuickItemText"].text = "None";
        }

        // nextItem
        if (statusData.quickItemList.Count > 1)
        {
            images["NextItemIcon"].color = Color.white;
            images["NextItemIcon"].sprite = statusData.quickItemList[(statusData.quickItemIndex + 1) % statusData.quickItemList.Count].Data.sprite;
            texts["NextItemText"].text = statusData.quickItemList[(statusData.quickItemIndex + 1) % statusData.quickItemList.Count].Data.instruction;
        }
        else
        {
            images["NextItemIcon"].color = Color.black;
            texts["NextItemText"].text = "None";
        }
    }

}
