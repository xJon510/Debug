using UnityEngine;

public class SwapEnemyType : MonoBehaviour
{
    [Header("Navigation References")]
    public RectTransform currentPanel;   // This panel (e.g., Worms)
    public RectTransform previousPanel;  // The panel to slide to when pressing "Prev"
    public RectTransform nextPanel;      // The panel to slide to when pressing "Next"

    [Header("Slide Settings")]
    public float slideDistance = 2000f;  // How far panels move left/right

    // Called by Prev Button
    public void ShowPreviousPanel()
    {
        if (previousPanel != null && currentPanel != null)
        {
            // Move current off-screen right
            currentPanel.anchoredPosition = new Vector2(slideDistance, 0f);

            // Bring previous into view
            previousPanel.anchoredPosition = Vector2.zero;
        }
    }

    // Called by Next Button
    public void ShowNextPanel()
    {
        if (nextPanel != null && currentPanel != null)
        {
            // Move current off-screen left
            currentPanel.anchoredPosition = new Vector2(-slideDistance, 0f);

            // Bring next into view
            nextPanel.anchoredPosition = Vector2.zero;
        }
    }
}
