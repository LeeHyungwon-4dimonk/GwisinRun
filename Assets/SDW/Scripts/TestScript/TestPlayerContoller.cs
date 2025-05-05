using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class TestPlayerContoller : MonoBehaviour
{
    [SerializeField] private Rigidbody m_rigidbody;
    [SerializeField] private float m_moveSpeed;
    [SerializeField] private GameObject m_pausedMenu;
    
    private Vector2 m_moveInput;
    private bool m_isFirstMove = true;
    

    private void Update()
    {
        Move();
        ActionKeys();
    }

    private void Move()
    {
        m_moveInput = GameManager.Instance.Input.MoveInput;
        m_rigidbody.velocity = new Vector3(m_moveInput.x * m_moveSpeed, 0, m_moveInput.y * m_moveSpeed);

        if (m_rigidbody.velocity.magnitude != 0 && m_isFirstMove)
        {
            GameManager.Instance.IsPaused = false;
            m_isFirstMove = false;
        }
    }

    private void ActionKeys()
    {
        //# 뛰기
        if (GameManager.Instance.Input.RunKeyPressed)
        {
            Debug.Log("Run Key Pressed");
        }
        if (GameManager.Instance.Input.RunKeyBeingHeld)
        {
            Debug.Log("Run Key Being Held");
        }
        if (GameManager.Instance.Input.RunKeyReleased)
        {
            Debug.Log("Run Key Released");
        }
        
        //# 앉기
        if (GameManager.Instance.Input.SitKeyPressed)
        {
            Debug.Log("Sit Key Pressed");
        }
        if (GameManager.Instance.Input.SitKeyBeingHeld)
        {
            Debug.Log("Sit Key Being Held");
        }
        if (GameManager.Instance.Input.SitKeyReleased)
        {
            Debug.Log("Sit Key Released");
        }
        
        //# 상호작용
        if (GameManager.Instance.Input.InteractionKeyPressed)
        {
            Debug.Log("상호작용 키");
        }
        
        //# Drop
        if (GameManager.Instance.Input.DropKeyPressed)
        {
            Debug.Log("Drop 키");
        }
        
        //# Items
        if (GameManager.Instance.Input.ItemsActionPressed)
        {
            Debug.Log($"{GameManager.Instance.Input.LastPressedKey + 1} 번째 슬롯 선택");
        }
        
        //# Pause
        if (GameManager.Instance.Input.PauseKeyPressed)
        {
            m_pausedMenu.SetActive(true);
        }

        if (GameManager.Instance.Input.JumpKeyPressed)
        {
            Debug.Log("Jump 키");
        }
    }
}
