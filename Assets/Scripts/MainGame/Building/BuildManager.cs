using UnityEngine;
using UnityEngine.InputSystem;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    [Header("Current Selection")]
    public GameObject selectedPrefab;     // set when player clicks a UI button
    private GameObject ghostObject;       // transparent preview while placing

    [Header("Placement Settings")]
    public LayerMask groundMask;
    public float cellSize = 1f;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (selectedPrefab == null) return;

        // Raycast mouse to ground
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mouseScreen);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask))
        {
            // Snap to grid
            int x = Mathf.RoundToInt(hit.point.x / cellSize);
            int z = Mathf.RoundToInt(hit.point.z / cellSize);
            Vector3 snappedPos = new Vector3(x * cellSize, 1, z * cellSize);

            // If ghost doesn't exist yet, spawn one
            if (ghostObject == null)
            {
                ghostObject = Instantiate(selectedPrefab, snappedPos, Quaternion.identity);
                SetGhostMaterial(ghostObject);
            }
            else
            {
                ghostObject.transform.position = snappedPos;
            }

            // Place on left click
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Instantiate(selectedPrefab, snappedPos, Quaternion.identity);
                Destroy(ghostObject);
                ghostObject = null;
                selectedPrefab = null; // reset selection
            }
        }
    }

    private void SetGhostMaterial(GameObject obj)
    {
        // make ghost semi-transparent (optional)
        foreach (var r in obj.GetComponentsInChildren<Renderer>())
        {
            r.material.color = new Color(0, 1, 0, 0.5f); // green transparent
        }
    }

    public void SelectBuilding(GameObject prefab)
    {
        // Called from UI buttons
        selectedPrefab = prefab;

        // cleanup old ghost if any
        if (ghostObject != null)
        {
            Destroy(ghostObject);
            ghostObject = null;
        }
    }
}
