using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button m_exitButton;
    [SerializeField] private Button m_mainMenuButton;

    [Header("Title Scene Name")]
    [SerializeField] private string m_sceneName;

    [Header("Background")]
    [SerializeField] private Image m_titleBackground;
    [SerializeField] private Image m_blackBackground;

    [Header("Current Time in Game")]
    [SerializeField] private GameObject m_currentTimeContainer;

    [Header("Update Key binding")]
    [SerializeField] private InputSetting m_inputSetting;

    [Header("UIs")]
    [SerializeField] private GameObject m_settingsMenu;
    [SerializeField] private GameObject m_soundsSettingDialog;
    [SerializeField] private GameObject m_graphicsSettingDialog;
    [SerializeField] private GameObject m_controlsSettingDialog;
    [SerializeField] private GameObject m_inventoryContainer;
    [SerializeField] private GameObject m_itemContainer;
    
    private void OnEnable()
    {
        GameManager.Instance.IsPaused = true;
        
        Cursor.lockState = CursorLockMode.None;
        
        m_titleBackground.gameObject.SetActive(true);
        m_blackBackground.gameObject.SetActive(true);
        m_currentTimeContainer.SetActive(false);
        m_inventoryContainer.SetActive(false);
        m_itemContainer.SetActive(false);
        
        m_exitButton.onClick.AddListener(GameManager.Instance.Exit);
        m_mainMenuButton.onClick.AddListener(() => GameManager.Instance.ChangeScene(m_sceneName));
    }

    public void Close()
    {
        m_exitButton.onClick.RemoveAllListeners();
        m_mainMenuButton.onClick.RemoveAllListeners();
        
        m_settingsMenu.SetActive(false);
        m_soundsSettingDialog.SetActive(false);
        m_graphicsSettingDialog.SetActive(false);
        m_controlsSettingDialog.SetActive(false);
        
        m_titleBackground.gameObject.SetActive(false);
        m_blackBackground.gameObject.SetActive(false);
        m_currentTimeContainer.SetActive(true);
        m_inventoryContainer.SetActive(true);
        m_itemContainer.SetActive(true);
        
        m_inputSetting.LoadBinding();
        
        GameManager.Instance.IsPaused = false;
        
        Cursor.lockState = CursorLockMode.Locked;
    }
}
