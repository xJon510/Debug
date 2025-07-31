using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

public class PlayerRotate : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera; // Assign your main camera in Inspector
    public LayerMask groundMask; // Set this to the layer your ground is on
    public LayerMask buildingsMask;

    void Update()
    {
        RotateToMouse();
    }

    void RotateToMouse()
    {
        // New Input System: get current mouse position
        Vector2 mousePos = UnityEngine.InputSystem.Pointer.current.position.ReadValue();

        Ray ray = mainCamera.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask))
        {
            Vector3 lookPos = hit.point - transform.position;
            lookPos.y = 0f; // Lock to Y-axis

            if (lookPos != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(lookPos);
            }
        }
    }
}
