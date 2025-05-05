using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GraphicsSetting : MonoBehaviour
{
    [Header("Graphics Setting")]
    [SerializeField] private PostProcessProfile m_UIBrightness;
    [SerializeField] private PostProcessProfile m_mainCameraProfile;
    [SerializeField] private PostProcessLayer m_postProcessLayer;
    
    private GraphicsModel m_defaultSetting;
    public GraphicsModel DefaultSetting => m_defaultSetting;
    private GraphicsModel m_setting;
    public GraphicsModel CurrentSetting => m_setting;
    
    private AutoExposure m_UIExposure;
    private AutoExposure m_mainCameraExposure;
    
    private Resolution[] m_resolutions;

    private bool m_isFirstTime = true;
    public bool IsFirstTime => m_isFirstTime;

    private void Awake()
    {
        m_defaultSetting = new GraphicsModel()
        {
            Brightness = 1f,
            Quality = 1,
            IsFullScreen = true
        };
    }

    private void Start()
    {
        m_UIBrightness.TryGetSettings(out m_UIExposure);
        m_mainCameraProfile.TryGetSettings(out m_mainCameraExposure);

        m_postProcessLayer = Camera.main.GetComponent<PostProcessLayer>();

        // SetBrightness(m_defaultSetting.Brightness);
        // SetFullScreen(m_defaultSetting.IsFullScreen);
        // SetQuality(m_defaultSetting.Quaility);
    }
    
    
    //! Graphics Setting Dialog
    public void SetBrightness(float brightness)
    {
        if (brightness != 0)
        {
            m_UIExposure.keyValue.value = brightness;
            m_mainCameraExposure.keyValue.value = brightness;
        }
        else
        {
            m_UIExposure.keyValue.value = 0.1f;
            m_mainCameraExposure.keyValue.value = 0.1f;
        }
        m_setting.Brightness= brightness;
    }

    public void SetFullScreen(bool isFullScreen)
    {
        m_setting.IsFullScreen = isFullScreen;
    }

    public void SetQuality(int qualityIndex)
    {
        m_setting.Quality = qualityIndex;
    }

    public void SetResolution(int resolutionIndex)
    {
        var resolution = m_resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void Reset()
    {
        SetBrightness(m_defaultSetting.Brightness);
        SetFullScreen(m_defaultSetting.IsFullScreen);
        SetQuality(m_defaultSetting.Quality);
        
        QualitySettings.SetQualityLevel(m_defaultSetting.Quality);
        Screen.fullScreen = m_defaultSetting.IsFullScreen;

        var currentResolution = Screen.currentResolution;
        Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);
        m_isFirstTime = false;
    }

    public void Apply()
    {
        QualitySettings.SetQualityLevel(m_setting.Quality);
        Screen.fullScreen = m_setting.IsFullScreen;
    }


    public List<string> GetResolution(out int index)
    {
        m_resolutions = Screen.resolutions;

        var options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < m_resolutions.Length; i++)
        {
            string option = $"{m_resolutions[i].width} x {m_resolutions[i].height} {m_resolutions[i].refreshRateRatio}Hz";

            options.Add(option);

            if (m_resolutions[i].width == Screen.width && m_resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        index = currentResolutionIndex;
        return options;
    }
}