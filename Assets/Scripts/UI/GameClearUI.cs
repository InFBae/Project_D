using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClearUI : PopUpUI
{
    protected override void Awake()
    {
        base.Awake();

        buttons["ReturnTitleButton"].onClick.AddListener(OnReturnTitleButton);
    }

    private void OnReturnTitleButton()
    {
        GameManager.Scene.LoadScene("Scenes/GameTitleScene", "");
    }
}