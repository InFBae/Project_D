using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class LevelUpPopUpUI : PopUpUI
{
    static bool isOpened = false;
    private int startEXP;
    public static bool IsOpened { get { return isOpened; } }
    
    protected override void Awake()
    {
        base.Awake();
        InitUI();
        startEXP = GameManager.Data.CurEXP;

        buttons["VitalityUpButton"].onClick.AddListener(OnVitalityUpButton);
        buttons["VitalityDownButton"].onClick.AddListener(OnVitalityDownButton);
        buttons["EnduranceUpButton"].onClick.AddListener(OnEnduranceUpButton);
        buttons["EnduranceDownButton"].onClick.AddListener(OnEnduranceDownButton);
        buttons["ResistanceUpButton"].onClick.AddListener(OnResistanceUpButton);
        buttons["ResistanceDownButton"].onClick.AddListener(OnResistanceDownButton);
        buttons["StrengthUpButton"].onClick.AddListener(OnStrengthUpButton);
        buttons["StrengthDownButton"].onClick.AddListener(OnStrengthDownButton);
        buttons["DexerityUpButton"].onClick.AddListener(OnDexerityUpButton);
        buttons["DexerityDownButton"].onClick.AddListener(OnDexerityDownButton);

        buttons["ApplyButton"].onClick.AddListener(OnApplyButton);
        buttons["RevertButton"].onClick.AddListener(OnRevertButton);
    }
    private void OnEnable()
    {
        isOpened = true;
        Cursor.lockState = CursorLockMode.None;
    }
    private void OnDisable()
    {
        isOpened = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void InitUI()
    {
        texts["VitalityCount"].text = GameManager.Data.PlayerStatusData.vitality.ToString();
        texts["VitalityUpgradeCostText"].text = (GameManager.Data.PlayerStatusData.vitality * 50).ToString();
        texts["EnduranceCount"].text = GameManager.Data.PlayerStatusData.endurance.ToString();
        texts["EnduranceUpgradeCostText"].text = (GameManager.Data.PlayerStatusData.endurance * 50).ToString();
        texts["ResistanceCount"].text = GameManager.Data.PlayerStatusData.resistance.ToString();
        texts["ResistanceUpgradeCostText"].text = (GameManager.Data.PlayerStatusData.resistance * 50).ToString();
        texts["StrengthCount"].text = GameManager.Data.PlayerStatusData.strength.ToString();
        texts["StrengthUpgradeCostText"].text = (GameManager.Data.PlayerStatusData.strength * 50).ToString();
        texts["DexerityCount"].text = GameManager.Data.PlayerStatusData.dexerity.ToString();
        texts["DexerityUpgradeCostText"].text = (GameManager.Data.PlayerStatusData.dexerity * 50).ToString();

        int.TryParse(texts["VitalityCount"].text, out vitalityCount);
        int.TryParse(texts["EnduranceCount"].text, out enduranceCount);
        int.TryParse(texts["ResistanceCount"].text, out resistanceCount);
        int.TryParse(texts["StrengthCount"].text, out strengthCount);
        int.TryParse(texts["DexerityCount"].text, out dexerityCount);
    }
    int vitalityCount;
    public void OnVitalityUpButton()
    {
        if ( int.TryParse(texts["VitalityCount"].text, out vitalityCount))
        {
            if (GameManager.Data.CurEXP >= vitalityCount * 50)
            {
                GameManager.Data.CurEXP -= vitalityCount * 50;
                vitalityCount++;
                texts["VitalityCount"].text = (vitalityCount).ToString();
                texts["VitalityUpgradeCostText"].text = (vitalityCount * 50).ToString();
            }
        }
        
    }
    public void OnVitalityDownButton()
    {
        if (int.TryParse(texts["VitalityCount"].text, out vitalityCount))
        {
            if (GameManager.Data.PlayerStatusData.vitality < vitalityCount)
            {
                vitalityCount--;
                GameManager.Data.CurEXP += vitalityCount * 50;
                texts["VitalityCount"].text = vitalityCount.ToString();
                texts["VitalityUpgradeCostText"].text = (vitalityCount * 50).ToString();
            }
        }
    }

    int enduranceCount;
    public void OnEnduranceUpButton()
    {
        
        if (int.TryParse(texts["EnduranceCount"].text, out enduranceCount))
        {
            if (GameManager.Data.CurEXP >= enduranceCount * 50)
            {
                GameManager.Data.CurEXP -= enduranceCount * 50;
                enduranceCount++;
                texts["EnduranceCount"].text = enduranceCount.ToString();
                texts["EnduranceUpgradeCostText"].text = (enduranceCount * 50).ToString();
            }
        }

    }
    public void OnEnduranceDownButton()
    {
        if (int.TryParse(texts["EnduranceCount"].text, out enduranceCount))
        {
            if (GameManager.Data.PlayerStatusData.vitality < enduranceCount)
            {
                enduranceCount--;
                GameManager.Data.CurEXP += enduranceCount * 50;
                texts["EnduranceCount"].text = enduranceCount.ToString();
                texts["EnduranceUpgradeCostText"].text = (enduranceCount * 50).ToString();
            }
        }
    }

    int resistanceCount;
    public void OnResistanceUpButton()
    {       
        if (int.TryParse(texts["ResistanceCount"].text, out resistanceCount))
        {
            if (GameManager.Data.CurEXP >= resistanceCount * 50)
            {
                GameManager.Data.CurEXP -= resistanceCount * 50;
                resistanceCount++;
                texts["ResistanceCount"].text = (resistanceCount).ToString();
                texts["ResistanceUpgradeCostText"].text = (resistanceCount * 50).ToString();
            }
        }

    }
    public void OnResistanceDownButton()
    {
        if (int.TryParse(texts["ResistanceCount"].text, out resistanceCount))
        {
            if (GameManager.Data.PlayerStatusData.vitality < resistanceCount)
            {
                resistanceCount--;
                GameManager.Data.CurEXP += resistanceCount * 50;
                texts["ResistanceCount"].text = resistanceCount.ToString();
                texts["ResistanceUpgradeCostText"].text = (resistanceCount * 50).ToString();
            }
        }
    }

    int strengthCount;
    public void OnStrengthUpButton()
    {       
        if (int.TryParse(texts["StrengthCount"].text, out strengthCount))
        {
            if (GameManager.Data.CurEXP >= strengthCount * 50)
            {
                GameManager.Data.CurEXP -= strengthCount * 50;
                strengthCount++;
                texts["StrengthCount"].text = (strengthCount).ToString();
                texts["StrengthUpgradeCostText"].text = (strengthCount * 50).ToString();
            }
        }

    }
    public void OnStrengthDownButton()
    {
        if (int.TryParse(texts["StrengthCount"].text, out strengthCount))
        {
            if (GameManager.Data.PlayerStatusData.vitality < strengthCount)
            {
                strengthCount--;
                GameManager.Data.CurEXP += strengthCount * 50;
                texts["StrengthCount"].text = strengthCount.ToString();
                texts["StrengthUpgradeCostText"].text = (strengthCount * 50).ToString();
            }
        }
    }

    int dexerityCount;
    public void OnDexerityUpButton()
    {
        if (int.TryParse(texts["DexerityCount"].text, out dexerityCount))
        {
            if (GameManager.Data.CurEXP >= dexerityCount * 50)
            {
                GameManager.Data.CurEXP -= dexerityCount * 50;
                dexerityCount++;
                texts["DexerityCount"].text = (dexerityCount).ToString();
                texts["DexerityUpgradeCostText"].text = (dexerityCount * 50).ToString();
            }
        }

    }
    public void OnDexerityDownButton()
    {
        if (int.TryParse(texts["DexerityCount"].text, out dexerityCount))
        {
            if (GameManager.Data.PlayerStatusData.vitality < dexerityCount)
            {
                dexerityCount--;
                GameManager.Data.CurEXP += dexerityCount * 50;
                texts["DexerityCount"].text = dexerityCount.ToString();
                texts["DexerityUpgradeCostText"].text = (dexerityCount * 50).ToString();
            }
        }
    }


    public void OnApplyButton()
    {
        GameManager.Data.PlayerStatusData.vitality = vitalityCount;
        GameManager.Data.PlayerStatusData.endurance = enduranceCount;
        GameManager.Data.PlayerStatusData.resistance = resistanceCount;
        GameManager.Data.PlayerStatusData.strength = strengthCount;
        GameManager.Data.PlayerStatusData.dexerity = dexerityCount;

        //GameManager.Data.SaveData();
        CloseUI();
    }

    public void OnRevertButton()
    {
        GameManager.Data.CurEXP = startEXP;
        CloseUI();
    }
}
