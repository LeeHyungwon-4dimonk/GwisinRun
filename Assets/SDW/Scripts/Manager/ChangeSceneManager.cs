using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ChangeSceneManager : MonoBehaviour
{
    [Header("Loading Scene UI")]
    [SerializeField] private GameObject m_loadingSceneCanvas;
    [SerializeField] private TMP_Text m_loadingSceneValue;
    [SerializeField] private CanvasGroup m_canvasGroup;
    private string m_sceneName;

    private void Awake()
    {
        m_loadingSceneCanvas.SetActive(false);
    }

    private void Start()
    {
        
    }
    
    public void SceneLoader(string sceneName)
    {
        m_loadingSceneCanvas.SetActive(true);
        SceneManager.sceneLoaded += OnSceneLoaded;

        m_sceneName = sceneName;
        
        StartCoroutine(LoadSceneProcess());
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == m_sceneName)
        {
            StartCoroutine(Fade(false));
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private IEnumerator LoadSceneProcess()
    {
        yield return StartCoroutine(Fade(true));

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(m_sceneName);
        asyncOperation.allowSceneActivation = false;

        float timer = 0f;

        while (!asyncOperation.isDone)
        {
            yield return null;
            if (!Mathf.Approximately(asyncOperation.progress, 0.9f))
            {
                m_loadingSceneValue.text = $"{asyncOperation.progress * 100f,3:F1}%";
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                float value = Mathf.Lerp(0.9f, 1f, asyncOperation.progress + 0.1f);
                m_loadingSceneValue.text = $"{value * 100f,3:F1}%";

                if (value >= 1f)
                {
                    asyncOperation.allowSceneActivation = true;
                    yield break;
                }
                
            }
        }
    }

    private IEnumerator Fade(bool isFadeIn)
    {
        float timer = 0f;
        while (timer <= 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime * 3f;
            
            // Fade In과 Fade Out 효과
            m_canvasGroup.alpha = isFadeIn ? Mathf.Lerp(0f, 1f, timer) : Mathf.Lerp(1f, 0f, timer);
        }

        if (!isFadeIn)
            m_loadingSceneCanvas.SetActive(false);
    }
}
