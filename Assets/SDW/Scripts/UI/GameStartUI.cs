using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartUI : MonoBehaviour
{
    [SerializeField] private GameObject m_inventoryContainer;
    
    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        m_inventoryContainer.SetActive(false);
    }


    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        GameManager.Instance.IsPaused = false;
        m_inventoryContainer.SetActive(true);
    }

    public void LoadScene(string sceneName)
    {
        GameManager.Instance.ChangeScene(sceneName);
    }
}
