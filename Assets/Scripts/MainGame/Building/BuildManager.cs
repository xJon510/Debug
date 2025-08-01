using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    [Header("Current Selection")]
    private BuildingData selectedBuilding;    // set when player clicks a UI button
    private GameObject ghostObject;           // transparent preview while placing

    [Header("Placement Settings")]
    public LayerMask groundMask;
    public float cellSize = 1f;

    public int rotation = 0; // 0=default, 1=90, 2=180, 3=270

    private InputAction cancelAction;
    private bool justPlaced = false;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        // Rotate Action
        InputAction rotateAction = new InputAction(binding: "<Keyboard>/r");
        rotateAction.performed += ctx => RotateBuilding();
        rotateAction.Enable();

        // Cancel action (esc + right click mapped in InputActions)
        cancelAction = new InputAction(binding: "<Keyboard>/escape");
        cancelAction.AddBinding("<Mouse>/rightButton");
        cancelAction.performed += ctx => CancelPlacement();
        cancelAction.Enable();
    }

    private void OnDisable()
    {
        cancelAction?.Disable();
    }

    private void Update()
    {
        if (selectedBuilding == null) return;

        // --- UI panel cancel check ---
        float mouseX = Mouse.current.position.ReadValue().x;
        if (mouseX <= 450f) // within 450px of the left side
        {
            if (ghostObject != null)
            {
                Destroy(ghostObject);
                ghostObject = null;
            }
            return; // don’t process placement
        }

        // Raycast mouse to ground
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mouseScreen);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask))
        {
            // Base snapped cell
            float x = Mathf.Floor(hit.point.x / cellSize);
            float z = Mathf.Floor(hit.point.z / cellSize);

            // Copy the size from the selected building
            Vector2Int size = selectedBuilding.size;

            // If rotated 90° or 270°, swap width/height
            if (rotation % 2 == 1)
                size = new Vector2Int(size.y, size.x);

            // Anchor: center of hovered cell
            float half = cellSize / 2f;
            Vector3 snappedPos = new Vector3(x * cellSize + half, 1, z * cellSize + half);

            Vector3 placementOffset = new Vector3(0, 0, 0);

            if (size.x == 2 && size.y == 1 || size.x == 1 && size.y == 2) // 2x1
            {
                switch (rotation % 4)
                {
                    case 0: placementOffset = new Vector3(cellSize / 2f, 0, 0); break; // X + 1.25
                    case 1: placementOffset = new Vector3(0, 0, cellSize / 2f); break; // Z + 1.25
                    case 2: placementOffset = new Vector3(-cellSize / 2f, 0, 0); break; // X - 1.25
                    case 3: placementOffset = new Vector3(0, 0, -cellSize / 2f); break; // Z - 1.25
                }
            }
            else if (size.x == 2 && size.y == 2) // 2x2
            {
                switch (rotation % 4)
                {
                    case 0: placementOffset = new Vector3(cellSize / 2f, 0, cellSize / 2f); break; // X+1.25, Z+1.25
                    case 1: placementOffset = new Vector3(-cellSize / 2f, 0, cellSize / 2f); break; // X-1.25, Z+1.25
                    case 2: placementOffset = new Vector3(-cellSize / 2f, 0, -cellSize / 2f); break; // X-1.25, Z-1.25
                    case 3: placementOffset = new Vector3(cellSize / 2f, 0, -cellSize / 2f); break; // X+1.25, Z-1.25
                }
            }

            if (ghostObject == null)
            {
                ghostObject = Instantiate(selectedBuilding.prefab, snappedPos, Quaternion.identity);
                SetGhostMaterial(ghostObject);
            }
            else
            {
                ghostObject.transform.position = snappedPos + placementOffset;
            }

            ghostObject.transform.rotation = Quaternion.Euler(0, rotation * 90f, 0);

            // Occupancy check (pass rotated size + rotation)
            bool inBounds = GridManager.Instance.AreCellsInBounds(snappedPos, selectedBuilding.size, rotation);
            bool occupied = GridManager.Instance.AreCellsOccupied(snappedPos, selectedBuilding.size, rotation);

            bool canPlace = inBounds && !occupied;
            SetGhostColor(canPlace ? Color.green : Color.red);

            if (canPlace && Mouse.current.leftButton.wasPressedThisFrame)
            {
                GameObject placed = Instantiate(selectedBuilding.prefab, snappedPos + placementOffset, Quaternion.Euler(0, rotation * 90f, 0));

                Collider rootCol = placed.GetComponent<Collider>();
                if (rootCol != null)
                    rootCol.enabled = true;

                // Generic call to OnPlaced
                Building building = placed.GetComponent<Building>();
                if (building != null)
                    building.OnPlaced();

                GridManager.Instance.OccupyCells(snappedPos, selectedBuilding.size, rotation);

                Destroy(ghostObject);
                ghostObject = null;
                selectedBuilding = null;

                justPlaced = true; // flag this frame
            }
        }
    }

    public void SelectBuilding(BuildingData data)
    {
        selectedBuilding = data;
        if (ghostObject != null) Destroy(ghostObject);
    }

    private void SetGhostMaterial(GameObject obj)
    {
        // make ghost semi-transparent (optional)
        foreach (var r in obj.GetComponentsInChildren<Renderer>())
        {
            r.material.color = new Color(0, 1, 0, 0.5f); // green transparent
        }
    }

    private void SetGhostColor(Color c)
    {
        foreach (var r in ghostObject.GetComponentsInChildren<Renderer>())
        {
            r.material.color = new Color(c.r, c.g, c.b, 0.5f); // semi-transparent
        }
    }

    private void RotateBuilding()
    {
        rotation = (rotation + 1) % 4; // cycle 0-3
        if (ghostObject != null)
        {
            ghostObject.transform.rotation = Quaternion.Euler(0, rotation * 90f, 0);
        }
    }

    private void CancelPlacement()
    {
        if (ghostObject != null)
        {
            Destroy(ghostObject);
            ghostObject = null;
        }
        selectedBuilding = null;
    }

    public bool IsPlacing() => selectedBuilding != null || justPlaced;

    private void LateUpdate()
    {
        // Clear flag at end of frame
        justPlaced = false;
    }
}
