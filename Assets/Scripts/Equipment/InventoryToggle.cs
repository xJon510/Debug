using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryToggle : MonoBehaviour
{
    public RectTransform inventoryPanel;
    public float slideDuration = 0.25f; // how fast the panel moves
    private Vector2 hiddenPos;
    private Vector2 shownPos;
    private Coroutine currentAnimation;

    void Start()
    {
        // Set up hidden and shown positions
        shownPos = new Vector2(inventoryPanel.anchoredPosition.x, 0f);
        hiddenPos = new Vector2(inventoryPanel.anchoredPosition.x, -2000f);

        // Start hidden
        inventoryPanel.anchoredPosition = hiddenPos;
    }

    public void OpenInventory()
    {
        if (currentAnimation != null) StopCoroutine(currentAnimation);
        currentAnimation = StartCoroutine(SlidePanel(inventoryPanel.anchoredPosition, shownPos));
    }

    public void CloseInventory()
    {
        if (currentAnimation != null) StopCoroutine(currentAnimation);
        currentAnimation = StartCoroutine(SlidePanel(inventoryPanel.anchoredPosition, hiddenPos));
    }

    private IEnumerator SlidePanel(Vector2 from, Vector2 to)
    {
        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            inventoryPanel.anchoredPosition = Vector2.Lerp(from, to, elapsed / slideDuration);
            elapsed += Time.unscaledDeltaTime; // unscaled so it works even if paused
            yield return null;
        }
        inventoryPanel.anchoredPosition = to;
    }
}
