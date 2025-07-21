using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;

    [Header("Gravity Settings")]
    public float gravity = -10f;
    public float verticalVelocity = 0f;
    public float groundedGravity = -1f;

    [Header("Dash Settings")]
    public float dashSpeed = 40f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private bool isDashing = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    private Vector3 dashDirection;

    [Header("Input")]
    public InputActionReference moveAction; // Player > Move (Vector2)
    public InputActionReference dashAction; // Player > Dash (Button)

    [Header("References")]
    public Camera mainCamera; // Assign this in the Inspector

    private Vector2 moveInput;
    private CharacterController controller;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        moveSpeed = StatManager.Instance.baseStats.moveSpeed;
        dashSpeed = StatManager.Instance.baseStats.dashSpeed;
        dashDuration = StatManager.Instance.baseStats.dashLength;
        dashCooldown = StatManager.Instance.baseStats.dashCooldown;
    }

    void OnEnable()
    {
        moveAction.action.Enable();
        dashAction.action.Enable();
        dashAction.action.performed += OnDash;
    }

    void OnDisable()
    {
        moveAction.action.Disable();
        dashAction.action.performed -= OnDash;
        dashAction.action.Disable();
    }

    void Update()
    {
        dashCooldownTimer -= Time.deltaTime;

        Vector3 moveDirection = GetMoveDirection();

        // Gravity
        if (controller.isGrounded)
        {
            verticalVelocity = groundedGravity;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        if (isDashing)
        {
            dashDirection = GetMoveDirection();
            if (dashDirection == Vector3.zero)
            {
                dashDirection = transform.forward;
            }

            controller.Move(dashDirection * dashSpeed * Time.deltaTime);
            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0f)
            {
                isDashing = false;
            }
        }
        else
        {
            moveDirection.y = verticalVelocity;
            controller.Move(moveDirection * moveSpeed * Time.deltaTime);
        }
    }

    private void OnDash(InputAction.CallbackContext ctx)
    {
        if (!isDashing && dashCooldownTimer <= 0f)
        {
            dashDirection = GetMoveDirection();
            if (dashDirection == Vector3.zero)
            {
                // Default to forward if no input
                dashDirection = mainCamera.transform.forward;
                dashDirection.y = 0f;
                dashDirection.Normalize();
            }

            isDashing = true;
            dashTimer = dashDuration;
            dashCooldownTimer = dashCooldown;
        }
    }

    private Vector3 GetMoveDirection()
    {
        moveInput = moveAction.action.ReadValue<Vector2>();

        Vector3 camForward = mainCamera.transform.forward;
        Vector3 camRight = mainCamera.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        return (camForward * moveInput.y + camRight * moveInput.x).normalized;
    }
}
