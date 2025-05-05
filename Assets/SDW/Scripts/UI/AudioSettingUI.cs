using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AudioSettingUI : MonoBehaviour
{
    [Header("Volume Setting")]
    [SerializeField] private TMP_Text m_BGMTextValue = null;
    [SerializeField] private Slider m_BGMSlider = null;
    [SerializeField] private TMP_Text m_SFXTextValue = null;
    [SerializeField] private Slider m_SFXSlider = null;
    [SerializeField] private Button m_resetButton = null;
    [SerializeField] private Button m_backButton = null;
    [SerializeField] private Button m_applyButton = null;

    private void Start()
    {
        SetBGMSliderAndText();
        SetSFXSliderAndText();
        m_BGMSlider.onValueChanged.AddListener(SoundDialogSetBGMVolume);
        m_SFXSlider.onValueChanged.AddListener(SoundDialogSetSFXVolume);
        m_resetButton.onClick.AddListener(SoundDialogResetButton);
        m_backButton.onClick.AddListener(SoundDialogBackButton);
        m_applyButton.onClick.AddListener(SoundDialogApplyButton);
    }
    
    private void SoundDialogSetBGMVolume(float volume)
    {
        float newVolume = GameManager.Instance.Audio.SetBGMVolume(volume);
        m_BGMSlider.value = newVolume;
        m_BGMTextValue.text = $"{(int)(newVolume * 100),0}%";
    }

    private void SoundDialogSetSFXVolume(float volume)
    {
        float newVolume = GameManager.Instance.Audio.SetSFXVolume(volume);
        m_SFXSlider.value = newVolume;
        m_SFXTextValue.text = $"{(int)(newVolume * 100),0}%";
    }
    
    private void SoundDialogApplyButton()
    {
        GameManager.Instance.Audio.ApplyVolume();
    }

    private void SoundDialogBackButton()
    {
        GameManager.Instance.Audio.RevertVolume();
        SetBGMSliderAndText();
        SetSFXSliderAndText();
    }

    private void SoundDialogResetButton()
    {
        GameManager.Instance.Audio.ResetVolume();
        SetBGMSliderAndText();
        SetSFXSliderAndText();
    }


    private void SetBGMSliderAndText()
    {
        float BGMVolume = GameManager.Instance.Audio.GetBGMVolume();
        m_BGMSlider.value = BGMVolume;
        m_BGMTextValue.text = $"{(int)(BGMVolume * 100),0}%";
    }

    private void SetSFXSliderAndText()
    {
        float SFXVolume = GameManager.Instance.Audio.GetSFXVolume();
        m_SFXSlider.value = SFXVolume;
        m_SFXTextValue.text = $"{(int)(SFXVolume * 100),0}%";
    }

    public void PlayButton()
    {
        GameManager.Instance.Audio.PlaySFX(GameManager.Instance.Audio.Click);
    }

}
