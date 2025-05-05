using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewGamePanelUI : MonoBehaviour
{
    [SerializeField] private Button m_newGameYesButton;
    
    [Header("New Game Scene Name")]
    [SerializeField] private string m_sceneName;
    
    void Start()
    {
        m_newGameYesButton.onClick.AddListener(() => GameManager.Instance.ChangeScene(m_sceneName));
    }

}
