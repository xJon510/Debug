using UnityEngine;
using UnityEngine.InputSystem;   // new input system
using UnityEngine.InputSystem.UI; // if using InputSystemUIInputModule
using UnityEngine.InputSystem.Utilities;

public class GridManager : MonoBehaviour
{
    public int width = 150;
    public int height = 150;
    public float cellSize = 1f;
    public LayerMask groundMask;

    [Header("Input Actions")]
    public InputActionReference clickAction; // drag "UI/Click" here in inspector

    private void OnEnable()
    {
        if (clickAction != null)
            clickAction.action.performed += OnClick;
    }

    private void OnDisable()
    {
        if (clickAction != null)
            clickAction.action.performed -= OnClick;
    }

    private void OnClick(InputAction.CallbackContext ctx)
    {
        Vector3? pos = GetMouseGridPosition();
        if (pos.HasValue)
        {
            Debug.Log("Clicked grid cell at: " + pos.Value);
        }
    }

    public Vector3? GetMouseGridPosition()
    {
        // Get mouse position from new Input System
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mouseScreen);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask))
        {
            int x = Mathf.RoundToInt(hit.point.x / cellSize);
            int z = Mathf.RoundToInt(hit.point.z / cellSize);
            return new Vector3(x * cellSize, 0, z * cellSize);
        }
        return null;
    }

    // Optional: show grid lines in Scene view
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        for (int x = 0; x <= width; x++)
        {
            Vector3 start = new Vector3(x * cellSize, 0, 0);
            Vector3 end = new Vector3(x * cellSize, 0, height * cellSize);
            Gizmos.DrawLine(transform.position + start, transform.position + end);
        }

        for (int z = 0; z <= height; z++)
        {
            Vector3 start = new Vector3(0, 0, z * cellSize);
            Vector3 end = new Vector3(width * cellSize, 0, z * cellSize);
            Gizmos.DrawLine(transform.position + start, transform.position + end);
        }
    }
}
