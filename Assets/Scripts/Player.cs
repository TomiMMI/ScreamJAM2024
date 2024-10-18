using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private CharacterController playerController;
    [SerializeField] private Transform playerCamera;
    private float moveSpeed = 5f;
    private float xRotation = 0;
    private float gravity = -9.8f;
    private float ySensitivity = 3f;
    private float xSensitivity = 3f;

    private void HandleMovement()
    {
        Vector2 moveDirection = InputManager.Instance.GetMoveInputValues();
        Vector3 moveValue = new Vector3(moveDirection.x * moveSpeed * Time.deltaTime, 0, moveDirection.y * moveSpeed * Time.deltaTime);
        RaycastHit hitInfo;
        Physics.Raycast(transform.position, Vector3.down, out hitInfo, 0.1f);
        Debug.DrawRay(transform.position, Vector3.down * 0.1f,Color.green);
        if (hitInfo.transform == null){
            Debug.Log("a");
            moveValue.y += gravity * Time.deltaTime;

        }
        playerController.Move(transform.TransformDirection(moveValue));
    }


    private void Start()
    {
        playerController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        HandleMovement();
        HandleLookRotation();
    }
    private void HandleLookRotation()
    {
        transform.eulerAngles = new Vector3(0, playerCamera.transform.eulerAngles.y, 0);
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
        Debug.DrawRay(transform.position, forward, Color.green);
        forward = playerCamera.TransformDirection(Vector3.forward) * 10;
        Debug.DrawRay(playerCamera.position, forward, Color.green);
    }
}
