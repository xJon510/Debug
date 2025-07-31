using UnityEngine;
using UnityEngine.InputSystem;   // new input system
using UnityEngine.InputSystem.UI; // if using InputSystemUIInputModule
using UnityEngine.InputSystem.Utilities;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    public int width = 150;
    public int height = 150;
    public float cellSize = 1f;
    public LayerMask groundMask;

    [Header("Input Actions")]
    public InputActionReference clickAction; // drag "UI/Click" here in inspector

    private HashSet<Vector2Int> occupiedCells = new HashSet<Vector2Int>();

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // ensure only one exists
            return;
        }
        Instance = this;
    }

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

        Gizmos.color = Color.red;
        foreach (var cell in occupiedCells)
        {
            Vector3 worldPos = new Vector3(
                cell.x * cellSize + cellSize / 2f,
                0.1f, // lift a bit above ground
                cell.y * cellSize + cellSize / 2f
            );
            Gizmos.DrawWireCube(worldPos, new Vector3(cellSize, 0.1f, cellSize));
        }
    }

    // Register a cell as occupied
    public bool AreCellsOccupied(Vector3 worldPos, Vector2Int size, int rotation)
    {
        Vector2Int baseCell = WorldToCell(worldPos);

        foreach (var cell in GetOccupiedCells(baseCell, size, rotation))
        {
            if (occupiedCells.Contains(cell))
                return true;
        }
        return false;
    }

    public void OccupyCells(Vector3 worldPos, Vector2Int size, int rotation)
    {
        Vector2Int baseCell = WorldToCell(worldPos);

        foreach (var cell in GetOccupiedCells(baseCell, size, rotation))
        {
            occupiedCells.Add(cell);
        }
    }

    private Vector2Int WorldToCell(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / cellSize);
        int z = Mathf.FloorToInt(pos.z / cellSize);
        return new Vector2Int(x, z);
    }

    private IEnumerable<Vector2Int> GetOccupiedCells(Vector2Int anchorCell, Vector2Int size, int rotation)
    {
        int rot = rotation % 4;

        for (int dx = 0; dx < size.x; dx++)
        {
            for (int dz = 0; dz < size.y; dz++)
            {
                Vector2Int offset;

                switch (rot)
                {
                    case 0: // default (facing right/down)
                        offset = new Vector2Int(dx, dz);
                        break;
                    case 1: // rotated 90° (facing up/right)
                        offset = new Vector2Int(-dz, dx);
                        break;
                    case 2: // rotated 180° (facing left/up)
                        offset = new Vector2Int(-dx, -dz);
                        break;
                    case 3: // rotated 270° (facing down/left)
                        offset = new Vector2Int(dz, -dx);
                        break;
                    default:
                        offset = new Vector2Int(dx, dz);
                        break;
                }

                yield return anchorCell + offset;
            }
        }
    }

}
