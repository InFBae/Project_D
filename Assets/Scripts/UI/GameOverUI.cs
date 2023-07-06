using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverUI : PopUpUI
{
    protected override void Awake()
    {
        base.Awake();

        buttons["ReturnTitleButton"].onClick.AddListener(OnReturnTitleButton);
        buttons["ReturnSavePointButton"].onClick.AddListener(OnReturnSavePointButton);
    }

    private void OnReturnTitleButton()
    {
        GameManager.Scene.LoadScene("Scenes/GameTitleScene", "");
    }
    private void OnReturnSavePointButton()
    {
        GameManager.Data.LoadData();

        string curScene = GameManager.Data.PlayerStatusData.savedScene;
        if (curScene == null || curScene == "")
        {
            GameManager.Scene.LoadScene("Scenes/DungeonMaps/Room_Library", "GameTitleScene");
        }
        else
        {
            GameManager.Scene.LoadScene(curScene, "GameTitleScene");
        }
    }

}
