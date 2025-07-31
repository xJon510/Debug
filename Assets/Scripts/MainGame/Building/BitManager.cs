using UnityEngine;
using UnityEngine.UI;
using TMPro; // if you’re using TextMeshPro

public class BitManager : MonoBehaviour
{
    public static BitManager Instance;

    [Header("Resource Settings")]
    [SerializeField] private int currentBits = 0;
    [SerializeField] private int maxBits = 1000; // base/default storage

    [Header("UI References")]
    [SerializeField] private Slider bitSlider;
    [SerializeField] private TMP_Text bitText; // swap with Text if not TMP

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

    public void AddBits(int amount)
    {
        currentBits = Mathf.Clamp(currentBits + amount, 0, maxBits);
        UpdateUI();
    }

    public void SpendBits(int amount)
    {
        currentBits = Mathf.Clamp(currentBits - amount, 0, maxBits);
        UpdateUI();
    }

    public void SetMaxBits(int newMax, bool keepRatio = true)
    {
        if (keepRatio)
        {
            // scale current bits to the new max
            float ratio = (float)currentBits / maxBits;
            maxBits = newMax;
            currentBits = Mathf.Clamp(Mathf.RoundToInt(ratio * maxBits), 0, maxBits);
        }
        else
        {
            maxBits = newMax;
            currentBits = Mathf.Clamp(currentBits, 0, maxBits);
        }
        UpdateUI();
    }

    // --- UI Sync ---
    private void UpdateUI()
    {
        if (bitSlider != null)
        {
            bitSlider.maxValue = maxBits;
            bitSlider.value = currentBits;
        }

        if (bitText != null)
        {
            bitText.text = $"{currentBits}/{maxBits}";
        }
    }

    // --- Getters ---
    public int GetCurrentBits() => currentBits;
    public int GetMaxBits() => maxBits;
}
