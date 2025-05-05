using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    //# 싱글턴
    private static GameManager m_instance;
    public static GameManager Instance => m_instance;
    
    public InputManager Input;
    public AudioSetting Audio;
    public GraphicsSetting Graphics;
    public Inventory Inventory;
    public ChangeSceneManager Scene;
    //todo 추후 리팩토링 시 검토
    // public UIManager UI;

    //todo 추후 사용 고민 게임 Time Attack에 사용할 Event
    // [NonSerialized] public UnityEvent OnGameStart;
    // [NonSerialized] public UnityEvent OnGameOver;
    [NonSerialized] public UnityEvent OnTimeChanged;

    //# 게임 Time Attack에 사용할 필드와 프로퍼티
    private float m_previousTime;
    public float PreviousTime => m_previousTime;

    private float m_currentTime;
    public float CurrentTime => m_currentTime;
    private float m_bestTime;
    public float BestTime => m_bestTime;

    //# GameOver 관련 필드와 프로퍼티, 추후 Event 방식으로 동작 여부 검토
    private bool m_isGameOver;
    public bool IsGameOver { get => m_isGameOver; set => m_isGameOver = value; }
    private bool m_isPaused = true;
    public bool IsPaused { get => m_isPaused; set => m_isPaused = value; }
    private bool m_isCleared;
    public bool IsCleared { get => m_isCleared; set => m_isCleared = value; }


    public static void CreateInstance()
    {
        if (m_instance == null)
        {
            var gameManagerPrefab = Resources.Load<GameManager>("GameManager");
            m_instance = Instantiate(gameManagerPrefab);
            DontDestroyOnLoad(m_instance);
        }
    }

    public static void ReleaseInstance()
    {
        if (m_instance != null)
        {
            Destroy(m_instance);
            m_instance = null;
        }
    }

    private void OnEnable()
    {
        // OnGameOver = new UnityEvent();
        // OnGameOver.AddListener(GameOver);
        // OnGameStart = new UnityEvent();
        OnTimeChanged = new UnityEvent();
    }

    private void OnDisable()
    {
        // OnGameStart.RemoveAllListeners();
        // OnGameOver.RemoveAllListeners();
        OnTimeChanged.RemoveAllListeners();
    }

    private void Update()
    {
        if (m_isGameOver || m_isPaused || m_isCleared)
            return;
        
        m_currentTime += Time.deltaTime;
        
        if (m_currentTime - m_previousTime >= 0.1f)
        {
            OnTimeChanged?.Invoke();
            m_previousTime = m_currentTime;
        }
    }

    public void GameStart(string sceneName)
    {
        m_currentTime = 0;
        m_previousTime = 0;
        m_isGameOver = false;
        m_isCleared = false;
        //# m_isPaused는 플레이어의 첫 키 입력이 있을 때 false
        m_isPaused = true;
        
        //# Read
        m_bestTime = PlayerPrefs.GetFloat(sceneName);
        
        if (m_bestTime <= 1.0f)
        {
            m_bestTime = float.MaxValue;
        }
    }

    public void GameOver()
    {
        m_isGameOver = true;
    }

    public void GameClear(string sceneName)
    {
        m_isCleared = true;
        
        if (m_currentTime < m_bestTime)
        {
            m_bestTime = m_currentTime;
            
            //# Write
            PlayerPrefs.SetFloat(sceneName, m_bestTime);
        }
        
        m_bestTime = PlayerPrefs.GetFloat(sceneName);
    }
    
    public void ChangeScene(string sceneName)
    {
        Scene.SceneLoader(sceneName);
        GameStart(sceneName);
    }

    public void Exit()
    {
        Application.Quit();
    }
}