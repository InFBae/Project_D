using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusInfoSceneUI : SceneUI
{
    protected override void Awake()
    {
        base.Awake();
    }

    public void SetHP(float HP)
    {
        sliders["HPBar"].value = HP;
    }

    public void SetSP(float SP)
    {
        sliders["SPBar"].value = SP;
    }
}
