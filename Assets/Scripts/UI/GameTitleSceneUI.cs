using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTitleSceneUI : SceneUI
{
    protected override void Awake()
    {
        base.Awake();

        Cursor.lockState = CursorLockMode.None;

        buttons["NewGameButton"].onClick.AddListener(OnNewGameButton);
        buttons["LoadGameButton"].onClick.AddListener(OnLoadGameButton);
        buttons["QuitButton"].onClick.AddListener(OnQuitButton);
    }

    private void OnNewGameButton()
    {
        GameManager.Data.ClearData();
        GameManager.Data.LoadData();
        GameManager.Scene.LoadScene("Scenes/DungeonMaps/Room_Library", "GameTitleScene");
    }
    private void OnLoadGameButton()
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
    private void OnQuitButton()
    {
        Application.Quit();
    }

}
