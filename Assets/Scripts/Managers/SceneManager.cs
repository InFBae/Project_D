using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class SceneManager : MonoBehaviour
{
    private LoadingUI loadingUI;

    private BaseScene curScene;
    public BaseScene CurScene
    {
        get
        {
            if (curScene == null)
                curScene = GameObject.FindObjectOfType<BaseScene>();

            return curScene;
        }
    }

    private void Awake()
    {
        LoadingUI loadingUI = Resources.Load<LoadingUI>("UI/LoadingUI");
        this.loadingUI = Instantiate(loadingUI);
        this.loadingUI.transform.SetParent(transform);
    }

    public void LoadScene(string sceneName, string exSceneName)
    {
        StartCoroutine(LoadingRoutine(sceneName, exSceneName));
    }

    IEnumerator LoadingRoutine(string sceneName, string exSceneName)
    {
        loadingUI.SetProgress(0f);
        loadingUI.FadeOut();
        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 0f;

        AsyncOperation oper = UnitySceneManager.LoadSceneAsync(sceneName);
        while (!oper.isDone)
        {
            loadingUI.SetProgress(Mathf.Lerp(0f, 0.5f, oper.progress));
            yield return null;
        }

        if (CurScene != null)
        {
            CurScene.LoadAsync(exSceneName);
            while (CurScene.progress < 1f)
            {
                loadingUI.SetProgress(Mathf.Lerp(0.5f, 1f, CurScene.progress));
                yield return null;
            }
        }

        loadingUI.SetProgress(1f);
        loadingUI.FadeIn();
        Time.timeScale = 1f;
        yield return new WaitForSecondsRealtime(0.5f);
    }
}
