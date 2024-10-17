using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float moveSpeed = 5f;
    private float playerHeight = 2f;
    private float playerRadius = 1f;
    private void HandleMovement()
    {
        Vector2 moveInput = InputManager.Instance.GetPlayerInputMap().Walking.Move.ReadValue<Vector2>();
        RaycastHit hit;
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        if(!Physics.CapsuleCast(transform.position, new Vector3(transform.position.x, transform.position.y + playerHeight, transform.position.z), playerRadius, moveDirection, out hit, 5))
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
    }

    private void Start()
    {
    }

    private void Update()
    {
        HandleMovement();
    }
}
