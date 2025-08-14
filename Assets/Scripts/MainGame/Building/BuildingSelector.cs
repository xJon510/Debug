using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class BuildingSelector : MonoBehaviour
{
    public LayerMask buildingMask; // assign to "Buildings" layer
    public OpenBuildPanel buildPanel;

    void Update()
    {
        // Don’t allow selecting buildings while in placement mode
        if (BuildManager.Instance != null && BuildManager.Instance.IsPlacing())
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, buildingMask))
            {
                Building building = hit.collider.GetComponentInParent<Building>();
                if (building != null)
                {
                    // Close build panel if open
                    if (buildPanel != null && buildPanel.isOpen)
                        buildPanel.CloseBuildPanelUI();

                    // Open this building's info panel
                    building.OpenInfoPanel();
                }
            }
        }
    }
}
