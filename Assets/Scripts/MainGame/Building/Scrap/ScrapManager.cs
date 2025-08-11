using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScrapManager : MonoBehaviour
{
    public static ScrapManager Instance;

    [Header("Resource Settings")]
    [SerializeField] private int currentScrap = 0;
    [SerializeField] private int maxScrap = 100; // base/default storage

    [Header("UI References")]
    [SerializeField] private Slider scrapSlider;
    [SerializeField] private TMP_Text scrapText;

    private void Awake()
    {
        // Singleton pattern so we can access from anywhere
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    // --- Resource Methods ---

    public void AddScrap(int amount)
    {
        currentScrap = Mathf.Clamp(currentScrap + amount, 0, maxScrap);
        UpdateUI();
    }

    public void SpendScrap(int amount)
    {
        currentScrap = Mathf.Clamp(currentScrap - amount, 0, maxScrap);
        UpdateUI();
    }

    public void SetMaxScrap(int newMax, bool keepRatio = true)
    {
        if (keepRatio)
        {
            // scale current scrap to the new max
            float ratio = (float)currentScrap / maxScrap;
            maxScrap = newMax;
            currentScrap = Mathf.Clamp(Mathf.RoundToInt(ratio * maxScrap), 0, maxScrap);
        }
        else
        {
            maxScrap = newMax;
            currentScrap = Mathf.Clamp(currentScrap, 0, maxScrap);
        }
        UpdateUI();
    }

    // --- UI Sync ---
    private void UpdateUI()
    {
        if (scrapSlider != null)
        {
            scrapSlider.maxValue = maxScrap;
            scrapSlider.value = currentScrap;
        }

        if (scrapText != null)
        {
            scrapText.text = $"{currentScrap}/{maxScrap}";
        }
    }

    // --- Getters ---
    public int GetCurrentScrap() => currentScrap;
    public int GetMaxScrap() => maxScrap;
}
