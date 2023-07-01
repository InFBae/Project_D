using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseScene : MonoBehaviour
{
    public float progress { get ; protected set; }
    protected abstract IEnumerator LoadingRoutine(string exScene);

    public void LoadAsync()
    {
        StartCoroutine(LoadingRoutine(null));
    }

    public void LoadAsync(string exScene)
    {
        StartCoroutine(LoadingRoutine(exScene));
    }
}
