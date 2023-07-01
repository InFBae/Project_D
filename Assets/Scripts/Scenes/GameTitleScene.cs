using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTitleScene : BaseScene
{
    protected override IEnumerator LoadingRoutine(string exScene)
    {
        progress = 0f;
        //
        yield return null;
        progress = 1f;       
    }

    public void OnGameStartButton()
    {
        string curScene = GameManager.Data.PlayerStatusData.savedScene;
        if (curScene == null || curScene == "" )
        {
            GameManager.Scene.LoadScene("Scenes/DungeonMaps/Room_Library", "GameTitleScene");
        }
        else
        {
            GameManager.Scene.LoadScene(curScene, "GameTitleScene");
        }
    }
}
