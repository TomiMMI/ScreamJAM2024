using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerInputMap playerInputMap;

    public event EventHandler OnInteractInputReceived;
    public event EventHandler OnInteractAlternateInputReceived;

    void Awake()
    {
        Instance = this;
        playerInputMap = new PlayerInputMap();
        playerInputMap.Enable();
    }

    private void Start()
    {
        playerInputMap.Walking.Interact.performed += Interact_performed;
        playerInputMap.Walking.InteractAlternate.performed += InteractAlternate_performed;
    }

    private void InteractAlternate_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractAlternateInputReceived?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractInputReceived?.Invoke(this, EventArgs.Empty);
    }
    public Vector2 GetMoveInputValues()
    {
        return playerInputMap.Walking.Move.ReadValue<Vector2>();
    }
    
    public Vector2 GetLookDirection()
    {
        return playerInputMap.Walking.Look.ReadValue<Vector2>();
    }
}
