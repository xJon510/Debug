using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;

    [Header("Input")]
    public InputActionReference moveAction; // Player > Move (Vector2)

    [Header("References")]
    public Camera mainCamera; // Assign this in the Inspector

    private Vector2 moveInput;
    private CharacterController controller;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void OnEnable()
    {
        moveAction.action.Enable();
    }

    void OnDisable()
    {
        moveAction.action.Disable();
    }

    void Update()
    {
        moveInput = moveAction.action.ReadValue<Vector2>();

        // Use camera forward/right for movement direction
        Vector3 camForward = mainCamera.transform.forward;
        Vector3 camRight = mainCamera.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDirection = (camForward * moveInput.y) + (camRight * moveInput.x);

        if (moveDirection.magnitude > 0.01f)
        {
            controller.Move(moveDirection.normalized * moveSpeed * Time.deltaTime);
        }
    }
}
