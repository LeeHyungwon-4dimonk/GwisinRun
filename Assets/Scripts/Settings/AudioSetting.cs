using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSetting : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer m_audioMixer;
    
    [Header("Audio Source")]
    [SerializeField] private AudioSource m_BGMSource;
    [SerializeField] private AudioSource m_SFXSource;

    [Header("Audio Clip")]
    public AudioClip Background;
    public AudioClip Click;

    private AudioModel m_initVolume = new AudioModel(0.5f, 0.5f);
    private AudioModel m_currentVolumeDB;

    private void Start()
    {
        VolumeInit();
        m_BGMSource.clip = Background;
        m_BGMSource.Play();
    }
    
    private void VolumeInit()
    {
        SetBGMVolume(m_initVolume.BGMVolume);
        SetSFXVolume(m_initVolume.SFXVolume);
        m_currentVolumeDB.BGMVolume = CalculateVolumeToDB(m_initVolume.BGMVolume);
        m_currentVolumeDB.SFXVolume = CalculateVolumeToDB(m_initVolume.SFXVolume);
    }
    
    private float CalculateVolumeToDB(float volume)
    {
        float dB;
        if (volume <= 0.001f)
            dB = -80f;
        else
            dB = Mathf.Log10(volume) * 20f;
        return dB;
    }

    private float CalculateDBToVolume(float dB)
    {
        float volume = Mathf.Pow(10, dB/ 20);
        //# 소수점 둘째 자리까지 반올림
        return Mathf.Round(volume * 100f) / 100f;
    }
    
    public float SetBGMVolume(float volume)
    {
        float dB = CalculateVolumeToDB(volume);
        m_audioMixer.SetFloat("BGM", dB);
        return CalculateDBToVolume(dB);
    }

    public float SetSFXVolume(float volume)
    {
        float dB = CalculateVolumeToDB(volume);
        m_audioMixer.SetFloat("SFX", dB);
        return CalculateDBToVolume(dB);
    }

    public float GetBGMVolume()
    {
        return  CalculateDBToVolume(m_currentVolumeDB.BGMVolume);
    }

    public float GetSFXVolume()
    {
        return CalculateDBToVolume(m_currentVolumeDB.SFXVolume);
    }

    public void ApplyVolume()
    {
        m_audioMixer.GetFloat("BGM", out m_currentVolumeDB.BGMVolume);
        m_audioMixer.GetFloat("SFX", out m_currentVolumeDB.SFXVolume);
    }

    public void RevertVolume()
    {
        m_audioMixer.SetFloat("BGM", m_currentVolumeDB.BGMVolume);
        m_audioMixer.SetFloat("SFX", m_currentVolumeDB.SFXVolume);
    }

    public void ResetVolume()
    {
        VolumeInit();
    }

    public void PlaySFX(AudioClip clip)
    {
        m_SFXSource.PlayOneShot(clip);
    }
}
