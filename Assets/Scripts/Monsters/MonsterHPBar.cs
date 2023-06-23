using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHPBar : MonoBehaviour
{
    public Monster owner;
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Start()
    {
        slider.maxValue = owner.MaxHP;
        slider.value = owner.CurHP;
        owner.OnHPChanged.AddListener(SetValue);
    }

    public void SetValue(float value)
    {
        slider.value = value;
        if(value <= 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}
