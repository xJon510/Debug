using System.Collections;
using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;

[RequireComponent(typeof(TMP_Text))]
public class TMP_EllipsisAnimator : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] TMP_Text target;

    [Header("Behavior")]
    [Tooltip("If the current label (rich text stripped) contains this word, we animate.")]
    [SerializeField] string triggerKeyword = "Processing";

    [Tooltip("Template used when animating. Use {0} where dots should appear.")]
    [SerializeField] string processingTemplate = "State: <color=#00FFFF>Processing{0}</color>";

    [Tooltip("How many dots to cycle through.")]
    [SerializeField] int maxDots = 3;

    [Tooltip("Seconds between dot steps.")]
    [SerializeField] float intervalSeconds = 0.4f;

    [Tooltip("If true, we’ll animate whenever we detect the keyword; if false, only when you call SetProcessing(true).")]
    [SerializeField] bool autoDetectFromText = false;

    // Optional manual control
    bool forceProcessing = false;

    // Cached to avoid GC
    string[] dotCache;
    int dotIndex = 0;
    Coroutine runner;

    void Awake()
    {
        if (!target) target = GetComponent<TMP_Text>();
        // Build dot cache: "", ".", "..", "..."
        maxDots = Mathf.Max(1, maxDots);
        dotCache = new string[maxDots + 1];
        dotCache[0] = "";
        for (int i = 1; i <= maxDots; i++)
            dotCache[i] = new string('.', i);
    }

    void OnEnable()
    {
        runner = StartCoroutine(Animate());
    }

    void OnDisable()
    {
        if (runner != null) StopCoroutine(runner);
        runner = null;
    }

    /// <summary>Manual control if you don’t want auto-detect.</summary>
    public void SetProcessing(bool isProcessing)
    {
        forceProcessing = isProcessing;
        if (!forceProcessing && autoDetectFromText == false)
        {
            // Reset to base template without dots if you want; no-op by default.
        }
    }

    IEnumerator Animate()
    {
        var wait = new WaitForSeconds(intervalSeconds);
        string lastRendered = null;

        while (true)
        {
            yield return wait;

            if (!target) continue;

            if (!forceProcessing) continue; // manual mode: do nothing unless active

            dotIndex = (dotIndex + 1) % (maxDots + 1);
            string dots = dotCache[dotIndex];

            string next = string.Format(processingTemplate, dots);

            // Avoid redundant assigns (less layout rebuilds, no flicker)
            if (!ReferenceEquals(lastRendered, next))
            {
                target.SetText(next);
                lastRendered = next;
            }
        }
    }

    private static string StripRichText(string input)
    {
        if (string.IsNullOrEmpty(input)) return "";
        return Regex.Replace(input, "<.*?>", string.Empty);
    }
}
