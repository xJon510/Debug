using UnityEngine;

public class OpenCombatPanel : MonoBehaviour
{
    [Header("References")]
    public CanvasGroup combatPanelGroup; // assign your CombatSelectionUI panel

    // Call this from your "Open" button OnClick()
    public void OpenCombatPanelUI()
    {
        if (combatPanelGroup != null)
        {
            combatPanelGroup.alpha = 1f;
            combatPanelGroup.interactable = true;
            combatPanelGroup.blocksRaycasts = true;
        }
    }

    // Call this from your "Close" button OnClick()
    public void CloseCombatPanelUI()
    {
        if (combatPanelGroup != null)
        {
            combatPanelGroup.alpha = 0f;
            combatPanelGroup.interactable = false;
            combatPanelGroup.blocksRaycasts = false;
        }
    }
}
