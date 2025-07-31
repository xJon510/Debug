using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingSelector : MonoBehaviour
{
    public LayerMask buildingMask; // assign to "Buildings" layer

    void Update()
    {
        // Don’t allow selecting buildings while in placement mode
        if (BuildManager.Instance != null && BuildManager.Instance.IsPlacing())
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, buildingMask))
            {
                Building building = hit.collider.GetComponentInParent<Building>();
                if (building != null)
                {
                    building.OpenInfoPanel();
                }
            }
        }
    }
}
