using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerInputMap playerInputMap;

    public event EventHandler<OnMoveInputReceivedEventArgs> OnMoveInputReceived;
    public class OnMoveInputReceivedEventArgs : EventArgs
    {
        public Vector2 moveValue;
    }

    public event EventHandler OnInteractInputReceived;

    void Awake()
    {
        Instance = this;
        playerInputMap = new PlayerInputMap();
    }

    private void Start()
    {
        playerInputMap.Enable();
        playerInputMap.Walking.Interact.performed += Interact_performed;
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
