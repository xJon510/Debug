using System.Collections;
using UnityEngine;

public class TabPanelSlider : MonoBehaviour
{
    [Header("Viewport (has RectMask2D)")]
    public RectTransform viewport;       // the masked parent

    [Header("Panels (children of viewport)")]
    public RectTransform infoPanel;
    public RectTransform upgradePanel;

    [Header("Slide Settings")]
    public float slideDuration = 0.25f;
    public AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public bool useViewportWidth = true;   // auto slide distance = viewport width
    public float slideDistance = 2000f;    // only used if useViewportWidth = false

    // Optional: prevent clicks on off-screen panel
    public CanvasGroup infoCG;
    public CanvasGroup upgradeCG;

    float Width => useViewportWidth && viewport ? viewport.rect.width : slideDistance;

    void Reset()
    {
        viewport = GetComponent<RectTransform>();
    }

    void Awake()
    {
        // Ensure both panels match viewport size & start positions
        float w = Width;
        if (infoPanel) infoPanel.anchoredPosition = Vector2.zero;        // visible
        if (upgradePanel) upgradePanel.anchoredPosition = new Vector2(w, 0f);  // off to right
        SetInteractable(infoCG, true);
        SetInteractable(upgradeCG, false);
    }

    public void ShowInfo()
    {
        if (!infoPanel || !upgradePanel) return;
        float w = Width;

        StopAllCoroutines();
        StartCoroutine(Slide(infoPanel, infoPanel.anchoredPosition, Vector2.zero));
        StartCoroutine(Slide(upgradePanel, upgradePanel.anchoredPosition, new Vector2(w, 0f)));

        SetInteractable(infoCG, true);
        SetInteractable(upgradeCG, false);
    }

    public void ShowUpgrade()
    {
        if (!infoPanel || !upgradePanel) return;
        float w = Width;

        StopAllCoroutines();
        StartCoroutine(Slide(infoPanel, infoPanel.anchoredPosition, new Vector2(-w, 0f)));
        StartCoroutine(Slide(upgradePanel, upgradePanel.anchoredPosition, Vector2.zero));

        SetInteractable(infoCG, false);
        SetInteractable(upgradeCG, true);
    }

    IEnumerator Slide(RectTransform rt, Vector2 from, Vector2 to)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / slideDuration;   // UI-friendly (ignores timescale)
            float k = ease.Evaluate(Mathf.Clamp01(t));
            rt.anchoredPosition = Vector2.LerpUnclamped(from, to, k);
            yield return null;
        }
        rt.anchoredPosition = to;
    }

    void SetInteractable(CanvasGroup cg, bool on)
    {
        if (!cg) return;
        cg.interactable = on;
        cg.blocksRaycasts = on;
        // Keep alpha as-is; RectMask2D will hide the off-screen panel visually
    }
}
