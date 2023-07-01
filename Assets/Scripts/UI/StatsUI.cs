using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsUI : BaseUI
{
    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        InitUI();
    }

    public void InitUI()
    {
        texts["VitalityCount"].text = GameManager.Data.PlayerStatusData.vitality.ToString();
        texts["EnduranceCount"].text = GameManager.Data.PlayerStatusData.endurance.ToString();
        texts["ResistanceCount"].text = GameManager.Data.PlayerStatusData.resistance.ToString();
        texts["StrengthCount"].text = GameManager.Data.PlayerStatusData.strength.ToString();
        texts["DexerityCount"].text = GameManager.Data.PlayerStatusData.dexerity.ToString();
    }
}
