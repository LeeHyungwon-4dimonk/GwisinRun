using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameClearUI : MonoBehaviour
{
    [Header("Time Text")]
    [SerializeField] private TMP_Text m_clearTime;
    [SerializeField] private TMP_Text m_bestTime;

    [Header("Main Menu")]
    [SerializeField] private Button m_mainMenuButton;
    [SerializeField] private string m_titleSceneName;
    
    [Header("Restart")]
    [SerializeField] private Button m_restartButton;
    [SerializeField] private string m_levelSceneName;
    
    [Header("Next Level")]
    [SerializeField] private Button m_nextLevelButton;
    [SerializeField] private string m_nextSceneName;
    
    [Header("Background")]
    [SerializeField] private Image m_titleBackground;
    [SerializeField] private Image m_blackBackground;
    
    [Header("Current Time in Game")]
    [SerializeField] private GameObject m_currentTimeContainer;
    
    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        GameManager.Instance.GameClear(m_levelSceneName);
            
        m_titleBackground.gameObject.SetActive(false);
        m_blackBackground.gameObject.SetActive(false);
        m_currentTimeContainer.SetActive(false);
        
        //todo 클리어 시간과 최단 시간이 같을 때(갱신) New 표시 여부 검토
        m_mainMenuButton.onClick.AddListener(() => GameManager.Instance.ChangeScene(m_titleSceneName));
        
        //# 재시작 시 GameStart 메서드를 호출하여 초기화
        m_restartButton.onClick.AddListener(() => GameManager.Instance.GameStart(m_levelSceneName));
        m_restartButton.onClick.AddListener(() => GameManager.Instance.ChangeScene(m_levelSceneName));
        
        //# 다음 레벨로 넘어갈 시 GameStart 메서드를 호출하여 초기화
        m_nextLevelButton.onClick.AddListener(() => GameManager.Instance.GameStart(m_nextSceneName));
        m_nextLevelButton.onClick.AddListener(() => GameManager.Instance.ChangeScene(m_nextSceneName));

        //# clear 시점의 CurrentTime이 클리어 시간
        m_clearTime.text = $"{GameManager.Instance.CurrentTime,6:F1}";
        m_bestTime.text = $"{GameManager.Instance.BestTime,6:F1}";
    }

    private void OnDisable()
    {
        m_mainMenuButton.onClick.RemoveAllListeners();
        m_restartButton.onClick.RemoveAllListeners();
        m_nextLevelButton.onClick.RemoveAllListeners();

        m_clearTime.text = "";
        m_bestTime.text = "";
        
        m_currentTimeContainer.SetActive(true);
    }
}
