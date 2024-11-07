using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private CharacterController playerController;
    [SerializeField] private Transform playerCamera;
    private float maxInteractibleDistance = 5f;
    private float moveSpeed = 5f;
    private float gravity = -9.8f;
    private GameObject lastSelected;

    private void Start()
    {
        playerController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        InputManager.Instance.OnInteractInputReceived += Instance_OnInteractInputReceived;
    }
    private void Update()
    {
        HandleMovement();
        HandleInteract();
        HandleLookRotation();
    }
    private void Instance_OnInteractInputReceived(object sender, System.EventArgs e)
    {
        if(lastSelected != null)
        {
            lastSelected.GetComponent<IInteractible>().Interact();
        }
    }
    private void HandleInteract()
    {
        GameObject selected = isSelectObjectInteractible();

        if (selected == null)
        {
            if(lastSelected != null)
            {
                lastSelected.GetComponent<LightSelected>().ToggleSelectedVisual();
                lastSelected = null;
            }
        }
        else if(selected != null)
        {
            if(lastSelected == null)
            {
                selected.GetComponent<LightSelected>().ToggleSelectedVisual();
                lastSelected = selected;
            }
            else
            {
                lastSelected.GetComponent<LightSelected>().ToggleSelectedVisual();
                selected.GetComponent<LightSelected>().ToggleSelectedVisual();
                lastSelected = selected;
            }
        }

    }

    private void HandleMovement()
    {
        Vector2 moveDirection = InputManager.Instance.GetMoveInputValues();
        Vector3 moveValue = new Vector3(moveDirection.x * moveSpeed * Time.deltaTime, 0, moveDirection.y * moveSpeed * Time.deltaTime);
        RaycastHit hitInfo;
        Physics.Raycast(transform.position, Vector3.down, out hitInfo, 0.1f);
        Debug.DrawRay(transform.position, Vector3.down * 0.1f,Color.green);
        if (hitInfo.transform == null){
            moveValue.y += gravity * Time.deltaTime;

        }
        playerController.Move(transform.TransformDirection(moveValue));
    }

    private void HandleLookRotation()
    {
        transform.eulerAngles = new Vector3(0, playerCamera.transform.eulerAngles.y, 0);
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
        Debug.DrawRay(transform.position, forward, Color.green);
        forward = playerCamera.TransformDirection(Vector3.forward) * 10;
        //Debug.DrawRay(playerCamera.position, forward, Color.green);
    }
    
    private GameObject isSelectObjectInteractible()
    {
        Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit info, maxInteractibleDistance);
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * 5f, Color.red);
        if (info.transform != null)
        {
            info.transform.TryGetComponent<IInteractible>(out IInteractible interactible);
            if (interactible != null)
            {
                return info.transform.gameObject;
            }
            return null;
        }
        return null;
    }
}
