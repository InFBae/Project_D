using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTitleScene : BaseScene
{
    private void Awake()
    {
        GameManager.Sound.Play("DefaultBGM", SoundManager.Sound.BGM);
    }
    protected override IEnumerator LoadingRoutine(string exScene)
    {
        // UI ·Îµù
        GameManager.UI.SceneLoadInit();
        GameManager.Pool.SceneLoadInit();
        progress = 0.5f;
        yield return null;

        progress = 1f;       
    }
}
