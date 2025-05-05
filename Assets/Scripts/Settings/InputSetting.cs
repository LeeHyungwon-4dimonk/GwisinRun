using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSetting : MonoBehaviour
{
    private PlayerInput m_playerInput;
    void Start()
    {
        m_playerInput = GetComponent<PlayerInput>();
        
        GameManager.Instance.Input.SetPlayerInput(m_playerInput);
        LoadBinding();
    }

    public void LoadBinding()
    {
        var rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
            m_playerInput.actions.LoadBindingOverridesFromJson(rebinds);
        
        foreach(var action in m_playerInput.actions)
        {
            if (action.name == "Interaction")
            {
                string keyValue = action.controls[0].displayName;
                PlayerPrefs.SetString("Interaction", keyValue);
                break;
            }
        }
    }
}
