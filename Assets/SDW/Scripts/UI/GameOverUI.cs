using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("Main Menu")]
    [SerializeField] private Button m_mainMenuButton;
    [SerializeField] private string m_titleSceneName;
    
    [Header("Restart")]
    [SerializeField] private Button m_restartButton;
    [SerializeField] private string m_levelSceneName;

    [Header("Exit")]
    [SerializeField] private Button m_exitButton;
    
    [Header("Background")]
    [SerializeField] private Image m_titleBackground;
    [SerializeField] private Image m_blackBackground;
    
    [Header("Current Time in Game")]
    [SerializeField] private GameObject m_currentTimeContainer;
    
    [Header("UIs")]
    [SerializeField] private GameObject m_inventoryContainer;
    [SerializeField] private GameObject m_playerStatusUI;
    [SerializeField] private GameObject m_interactionContainer;
    [SerializeField] private GameObject m_centerPoint;
    
    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        
        m_titleBackground.gameObject.SetActive(false);
        m_blackBackground.gameObject.SetActive(false);
        m_currentTimeContainer.SetActive(false);
        
        m_inventoryContainer.SetActive(false);
        m_playerStatusUI.SetActive(false);
        m_interactionContainer.SetActive(false);
        m_centerPoint.SetActive(false);
        
        m_restartButton.onClick.AddListener(() => GameManager.Instance.ChangeScene(m_levelSceneName));
        m_mainMenuButton.onClick.AddListener(() => GameManager.Instance.ChangeScene(m_titleSceneName));
        m_exitButton.onClick.AddListener(GameManager.Instance.Exit);
        
        GameManager.Instance.GameOver();
    }

    private void OnDisable()
    {
        m_mainMenuButton.onClick.RemoveAllListeners();
        m_restartButton.onClick.RemoveAllListeners();
        m_exitButton.onClick.RemoveAllListeners();
        
        m_inventoryContainer.SetActive(true);
        m_playerStatusUI.SetActive(true);
        m_interactionContainer.SetActive(true);
        m_centerPoint.SetActive(true);
        
        m_currentTimeContainer.SetActive(true);
    }

    // public void LoadScene(string sceneName)
    // {
    //     GameManager.Instance.ChangeScene(sceneName);
    // }
}
