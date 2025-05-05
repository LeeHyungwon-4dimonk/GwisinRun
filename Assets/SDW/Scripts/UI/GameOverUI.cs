using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("Restart")]
    [SerializeField] private Button m_restartButton;
    [SerializeField] private string m_levelSceneName;

    [Header("Main Menu")]
    [SerializeField] private Button m_mainMenuButton;
    [SerializeField] private string m_titleSceneName;
    
    [Header("Exit")]
    [SerializeField] private Button m_exitButton;
    
    [Header("Background")]
    [SerializeField] private Image m_titleBackground;
    [SerializeField] private Image m_blackBackground;
    
    [Header("Current Time in Game")]
    [SerializeField] private GameObject m_currentTimeContainer;
    
    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        GameManager.Instance.GameOver();
        
        m_titleBackground.gameObject.SetActive(false);
        m_blackBackground.gameObject.SetActive(false);
        m_currentTimeContainer.SetActive(false);
        
        m_restartButton.onClick.AddListener(() => GameManager.Instance.ChangeScene(m_levelSceneName));
        m_restartButton.onClick.AddListener(() => GameManager.Instance.GameStart(m_levelSceneName));
        
        m_mainMenuButton.onClick.AddListener(() => GameManager.Instance.ChangeScene(m_titleSceneName));
        
        m_exitButton.onClick.AddListener(GameManager.Instance.Exit);
    }

    private void OnDisable()
    {
        m_mainMenuButton.onClick.RemoveAllListeners();
        m_restartButton.onClick.RemoveAllListeners();
        m_exitButton.onClick.RemoveAllListeners();
        
        m_currentTimeContainer.SetActive(true);
    }

    public void LoadScene(string sceneName)
    {
        GameManager.Instance.ChangeScene(sceneName);
    }
}
