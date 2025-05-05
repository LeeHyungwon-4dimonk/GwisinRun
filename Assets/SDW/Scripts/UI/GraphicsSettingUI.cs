using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsSettingUI : MonoBehaviour
{
    [Header("Graphics UI Setting")]
    [SerializeField] private TMP_Text m_brightnessTextValue = null;
    [SerializeField] private Slider m_brightnessSlider = null;
    [SerializeField] private Toggle m_fullscreenToggle;
    [SerializeField] private TMP_Dropdown m_resolutionDropdown;
    [SerializeField] private TMP_Dropdown m_qualityDropdown;
    [SerializeField] private Button m_resetButton;
    [SerializeField] private Button m_applyButton;
    
    private Resolution[] m_resolutions;

    private void OnEnable()
    {
        m_brightnessSlider.onValueChanged.AddListener(GameManager.Instance.Graphics.SetBrightness);
        m_brightnessSlider.onValueChanged.AddListener(GraphicsDialogSetBrightness);
        
        m_fullscreenToggle.onValueChanged.AddListener(GameManager.Instance.Graphics.SetFullScreen);
        
        m_resolutionDropdown.onValueChanged.AddListener(GameManager.Instance.Graphics.SetResolution);
        
        m_qualityDropdown.onValueChanged.AddListener(GameManager.Instance.Graphics.SetQuality);
        
        m_resetButton.onClick.AddListener(GameManager.Instance.Graphics.Reset);
        m_resetButton.onClick.AddListener(GraphicsDialogResetButton);
        
        m_applyButton.onClick.AddListener(GameManager.Instance.Graphics.Apply);
    }

    private void OnDisable()
    {
        m_brightnessSlider.onValueChanged.RemoveAllListeners();
        m_fullscreenToggle.onValueChanged.RemoveAllListeners();
        m_resolutionDropdown.onValueChanged.RemoveAllListeners();
        m_qualityDropdown.onValueChanged.RemoveAllListeners();
        m_resetButton.onClick.RemoveAllListeners();
        m_applyButton.onClick.RemoveAllListeners();
        
    }

    private void Start()
    {
        if (GameManager.Instance.Graphics.IsFirstTime)
        {
            GameManager.Instance.Graphics.Reset();
        }
        InitUISetting();
    }

    private void InitUISetting()
    {
        GraphicsModel currentSetting = GameManager.Instance.Graphics.CurrentSetting;
        m_brightnessSlider.value = currentSetting.Brightness;
        m_brightnessTextValue.text = (currentSetting.Brightness * 100).ToString("0");

        m_qualityDropdown.value = currentSetting.Quality;

        m_fullscreenToggle.isOn = currentSetting.IsFullScreen;

        GetResolution();
    }

    private void GetResolution()
    {
        m_resolutionDropdown.ClearOptions();
        List<string> options = GameManager.Instance.Graphics.GetResolution(out int currentResolutionIndex);
        m_resolutionDropdown.AddOptions(options);
        m_resolutionDropdown.value = currentResolutionIndex;
        m_resolutionDropdown.RefreshShownValue();
    }

    private void GraphicsDialogSetBrightness(float brightness)
    {
        m_brightnessTextValue.text = (brightness * 100).ToString("0");
    }
    

    private void GraphicsDialogResetButton()
    {
        InitUISetting();
    }
}
