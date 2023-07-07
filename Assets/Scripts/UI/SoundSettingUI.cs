using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSettingUI : BaseUI
{
    float masterVolumeRevertValue;
    float bgmVolumeRevertValue;
    float effectVolumeRevertValue;
    protected override void Awake()
    {
        base.Awake();

        sliders["MasterVolumeSlider"].onValueChanged.AddListener(OnMasterVolumeChanged);
        sliders["BGMVolumeSlider"].onValueChanged.AddListener(OnBGMVolumeChanged);
        sliders["EffectVolumeSlider"].onValueChanged.AddListener(OnEffectVolumeChanged);
        
        buttons["ApplyButton"].onClick.AddListener(OnApplyButton);
        buttons["RevertButton"].onClick.AddListener(OnRevertButton);
    }

    private void OnEnable()
    {
        InitUI();
    }

    public void InitUI()
    {
        masterVolumeRevertValue = GameManager.Sound.GetAudioVolume("Master");
        bgmVolumeRevertValue = GameManager.Sound.GetAudioVolume("BGM");
        effectVolumeRevertValue = GameManager.Sound.GetAudioVolume("Effect");
        sliders["MasterVolumeSlider"].value = masterVolumeRevertValue;
        sliders["BGMVolumeSlider"].value = bgmVolumeRevertValue;
        sliders["EffectVolumeSlider"].value = effectVolumeRevertValue;
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
    public void OnApplyButton()
    {
        InitUI();
        GameManager.Data.SaveSoundSetting();
        CloseUI();
    }

    public void OnRevertButton()
    {
        GameManager.Sound.SetAudioVolume("Master", masterVolumeRevertValue);
        GameManager.Sound.SetAudioVolume("BGM", bgmVolumeRevertValue);
        GameManager.Sound.SetAudioVolume("Effect", effectVolumeRevertValue);
        InitUI();
    }
}
