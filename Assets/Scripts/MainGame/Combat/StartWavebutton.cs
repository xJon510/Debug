using UnityEngine;

public class StartWaveButton : MonoBehaviour
{
    [Header("UI References")]
    public CanvasGroup combatSelectionUI;
    public CanvasGroup playerBuildUI;
    public CanvasGroup playerCombatUI;

    [Header("Player References")]
    public GameObject player; // assign your player object
    public MonoBehaviour fireBulletScript; // drag the FireBullet.cs script here
    public GameObject playerCamera;
    public MonoBehaviour buildingSelectionScript;

    public void StartWave()
    {
        // Hide selection/build UI
        SetCanvasGroupActive(combatSelectionUI, false);
        SetCanvasGroupActive(playerBuildUI, false);

        // Show combat UI
        SetCanvasGroupActive(playerCombatUI, true);

        // Enable firing
        if (fireBulletScript != null)
            fireBulletScript.enabled = true;
        else
            Debug.LogWarning("FireBullet script not assigned!");

        // Enable firing
        if (buildingSelectionScript != null)
            buildingSelectionScript.enabled = false;
        else
            Debug.LogWarning("BuildingSelection script not assigned!");

        // Start the wave
        EnemyManager.Instance.BeginWaves();
    }

    public void EndWave()
    {
        // Hide combat UI
        SetCanvasGroupActive(playerCombatUI, false);

        // Show build UI
        SetCanvasGroupActive(playerBuildUI, true);

        // Disable firing
        if (fireBulletScript != null)
            fireBulletScript.enabled = false;

        // Re-enable building selection
        if (buildingSelectionScript != null)
            buildingSelectionScript.enabled = true;

        Debug.Log("Wave ended -> Build phase restored");
    }

    private void SetCanvasGroupActive(CanvasGroup group, bool active)
    {
        if (group == null) return;

        group.alpha = active ? 1 : 0;
        group.interactable = active;
        group.blocksRaycasts = active;
    }
}
