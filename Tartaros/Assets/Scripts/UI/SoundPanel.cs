using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundPanel : MonoBehaviour
{
    [SerializeField] public Slider BGMSlider;
    [SerializeField] public Slider SFXSlider;
    
    private void OnEnable()
    {
        BGMSlider.value = SoundManager.Instance.MusicVolume;
        SFXSlider.value = SoundManager.Instance.SoundEffectVolume;
    }
}
