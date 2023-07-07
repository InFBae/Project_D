using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSettingUI : BaseUI
{
    protected override void Awake()
    {
        base.Awake();

        sliders["MasterVolumeSlider"].onValueChanged.AddListener(OnMasterVolumeChanged);
        sliders["BGMVolumeSlider"].onValueChanged.AddListener(OnBGMVolumeChanged);
        sliders["EffectVolumeSlider"].onValueChanged.AddListener(OnEffectVolumeChanged);
    }

    private void OnEnable()
    {
        InitUI();
    }

    public void InitUI()
    {
        sliders["MasterVolumeSlider"].value = GameManager.Sound.GetAudioVolume("Master");
        sliders["BGMVolumeSlider"].value = GameManager.Sound.GetAudioVolume("BGM");
        sliders["EffectVolumeSlider"].value = GameManager.Sound.GetAudioVolume("Effect");
    }

    private void OnMasterVolumeChanged(float value)
    {
        GameManager.Sound.SetAudioVolume("Master", value);
    }    
    
    private void OnBGMVolumeChanged(float value)
    {
        GameManager.Sound.SetAudioVolume("BGM", value);
    }    
    
    private void OnEffectVolumeChanged(float value)
    {
        GameManager.Sound.SetAudioVolume("Effect", value);
    }
}
