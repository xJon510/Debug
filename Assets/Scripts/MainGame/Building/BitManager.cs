using UnityEngine;
using UnityEngine.UI;
using TMPro; // if you’re using TextMeshPro
using System;
using System.Collections.Generic;

public class BitManager : MonoBehaviour
{
    public static BitManager Instance;
    public event Action<int> OnBitsChanged;

    [Header("Resource Settings")]
    [SerializeField] private int currentBits = 0;
    [SerializeField] private int maxBits = 1000; // base/default storage

    [Header("UI References")]
    [SerializeField] private Slider bitSlider;
    [SerializeField] private TMP_Text bitText; // swap with Text if not TMP

    private List<BitMiner> activeMiners = new List<BitMiner>();

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
        OnBitsChanged?.Invoke(currentBits);
    }

    private void Update()
    {
        float delta = Time.deltaTime;
        for (int i = 0; i < activeMiners.Count; i++)
        {
            activeMiners[i].TickProduction(delta);
        }
    }

    // --- Miner Registration ---
    public void RegisterMiner(BitMiner miner)
    {
        if (!activeMiners.Contains(miner))
            activeMiners.Add(miner);
    }

    public void UnregisterMiner(BitMiner miner)
    {
        if (activeMiners.Contains(miner))
            activeMiners.Remove(miner);
    }

    // --- Resource Methods ---

    public void AddBits(int amount)
    {
        currentBits = Mathf.Clamp(currentBits + amount, 0, maxBits);
        UpdateUI();
        OnBitsChanged?.Invoke(currentBits);
    }

    public void SpendBits(int amount)
    {
        currentBits = Mathf.Clamp(currentBits - amount, 0, maxBits);
        UpdateUI();
        OnBitsChanged?.Invoke(currentBits);
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
        OnBitsChanged?.Invoke(currentBits);
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

    public int CollectAllMiners()
    {
        int total = 0;
        // assuming you have a List<BitMiner> activeMiners
        for (int i = 0; i < activeMiners.Count; i++)
        {
            int before = Mathf.FloorToInt(activeMiners[i].currentStored);
            if (before > 0)
            {
                activeMiners[i].Collect();
                total += before;
            }
        }
        return total;
    }
}
