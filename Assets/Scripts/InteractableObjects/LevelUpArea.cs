using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelUpArea : MonoBehaviour, IInteractable
{
    Canvas canvas;
    [SerializeField] string curScene;
    [SerializeField] int savePointIndex;

    private void Awake()
    {
        canvas = GetComponentInChildren<Canvas>();
        canvas.enabled = false;
    }
    public void Enter()
    {
        if (canvas != null)
        {
            canvas.enabled = true;
        }
    }

    public void Exit()
    {
        if (canvas != null)
        {
            canvas.enabled = false;
        }
    }
    public void Interact()
    {
        if (!LevelUpPopUpUI.IsOpened)
        {
            LevelUpPopUpUI levelUpPopUpUI = GameManager.UI.ShowPopUpUI<LevelUpPopUpUI>("UI/LevelUpPopUpUI");
            levelUpPopUpUI.curScene = curScene;
            levelUpPopUpUI.spawnPointIndex = savePointIndex;
        }           
    }
}
