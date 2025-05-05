using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class InputManager : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public bool RunKeyPressed { get; private set; }
    public bool RunKeyBeingHeld { get; private set; }
    public bool RunKeyReleased { get; private set; }
    public bool SitKeyPressed { get; private set; }
    public bool SitKeyBeingHeld { get; private set; }
    public bool SitKeyReleased { get; private set; }
    public bool InteractionKeyPressed { get; private set; }
    public bool DropKeyPressed { get; private set; }
    public bool UseItemKeyPressed { get; private set; }
    public bool ItemsActionPressed { get; private set; }
    public int LastPressedKey { get => int.Parse(m_lastPressedKey.name) - 1; }
    public bool PauseKeyPressed { get; private set; }
    public bool JumpKeyPressed { get; private set; }

    private PlayerInput m_playerInput;
    
    private InputAction m_moveAction;
    private InputAction m_runAction;
    private InputAction m_sitAction;
    private InputAction m_interactionAction;
    private InputAction m_dropAction;
    private InputAction m_useItemAction;
    private InputAction m_itemsAction;
    private InputAction m_pauseAction;
    private InputAction m_jumpAction;
    private bool m_isUpdateable;
    private KeyControl m_lastPressedKey;
    private double m_lastPressedTime = double.MinValue;

    private void Awake()
    {
    }

    private void Update()
    {
        if(m_isUpdateable == true)
            UpdateInputs();
    }

    private void SetupInputActions()
    {
        m_moveAction = m_playerInput.actions["Move"];
        m_runAction = m_playerInput.actions["Run"];
        m_sitAction = m_playerInput.actions["Sit"];
        m_interactionAction = m_playerInput.actions["Interaction"];
        m_dropAction = m_playerInput.actions["Drop"];
        m_useItemAction = m_playerInput.actions["UseItem"];
        m_itemsAction = m_playerInput.actions["Items"];
        m_pauseAction = m_playerInput.actions["Pause"];
        m_jumpAction = m_playerInput.actions["Jump"];

        //# Pressed
        // m_itemsAction.started += OnInteractionStarted;
        //# BeingHeld
        // m_itemsAction.performed
        //# Released
        // m_itemsAction.canceled
    }

    public void SetPlayerInput(PlayerInput playerInput)
    {
        m_playerInput = playerInput;
        SetupInputActions();
        m_isUpdateable = true;
    }

    private void UpdateInputs()
    {
        MoveInput = m_moveAction.ReadValue<Vector2>();
        RunKeyPressed = m_runAction.WasPressedThisFrame();
        RunKeyBeingHeld = m_runAction.IsPressed();
        RunKeyReleased = m_runAction.WasReleasedThisFrame();
        SitKeyPressed = m_sitAction.WasPressedThisFrame();
        SitKeyBeingHeld = m_sitAction.IsPressed();
        SitKeyReleased = m_sitAction.WasReleasedThisFrame();
        InteractionKeyPressed = m_interactionAction.WasPressedThisFrame();
        DropKeyPressed = m_dropAction.WasPressedThisFrame();
        UseItemKeyPressed = m_useItemAction.WasPressedThisFrame();
        PauseKeyPressed = m_pauseAction.WasPressedThisFrame();
        JumpKeyPressed= m_jumpAction.WasPressedThisFrame();


        if (m_itemsAction.WasPressedThisFrame())
        {
            foreach (var control in m_itemsAction.controls)
            {
                var keyControl = control as KeyControl;
                if (keyControl !=null && keyControl.wasPressedThisFrame)
                {
                    float now = Time.realtimeSinceStartup;
                    if (now >= m_lastPressedTime)
                    {
                        m_lastPressedKey = keyControl;
                        m_lastPressedTime = now;
                    }
                }
            }
        }
        ItemsActionPressed = m_itemsAction.WasPressedThisFrame();
    }
    //
    // private void OnInteractionStarted(InputAction.CallbackContext context)
    // {
    //     var control = context.control;
    //
    //     int index = int.Parse(control.name) - 1;
    //     ItemKeyPressed[index] = true;
    // }
}
