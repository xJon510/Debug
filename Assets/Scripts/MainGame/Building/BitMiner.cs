using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BitMinerLevelStats
{
    [Min(1)] public int capacity = 200;
    [Min(0)] public int productionPerHour = 90;

    [Header("Upgrade Cost To Reach THIS Level")]
    [Min(0)] public int upgradeCostBits = 200;
    [Min(0)] public int upgradeCostScrap = 10;

    [Tooltip("Seconds required to upgrade from the previous level to THIS level.")]
    [Min(0)] public int upgradeTimeSeconds = 300;

    [TextArea] public string levelNote;
}

public class BitMiner : Building
{
    [Header("Levels")]
    [Tooltip("Level 1 should describe the base stats. Each element represents the stats AT that level.")]
    public List<BitMinerLevelStats> levels = new List<BitMinerLevelStats>
    {
        new BitMinerLevelStats
        {
            capacity = 200,
            productionPerHour = 90,
            upgradeCostBits = 0,   // Level 1 usually has no cost (starting level)
            upgradeCostScrap = 0,
            upgradeTimeSeconds = 0,
            levelNote = "Base production miner."
        },
        new BitMinerLevelStats
        {
            capacity = 400,
            productionPerHour = 150,
            upgradeCostBits = 200,
            upgradeCostScrap = 10,
            upgradeTimeSeconds = 300,
            levelNote = "More efficient memory taps."
        }
    };

    [Tooltip("1-based current level for readability in the inspector.")]
    [Min(1)] public int currentLevel = 1;

    [Header("Upgrade State")]
    public bool isUpgrading = false;
    public float upgradeTimeRemaining = 0f;
    private float upgradeEndTimestamp = -1f;

    [Header("Description")]
    [TextArea]
    public string description =
        "An automated process that digs into the digital substrate, extracting stray \"bits\" from memory leaks. Essential for fueling your defenses against the Bugs.";

    [Header("Production Tracking")]
    public float currentStored = 0f;

    private CanvasGroup infoCanvas;

    // -- Convenience getters for current/next stats --------------------------

    public bool IsMaxLevel => levels == null || levels.Count == 0 || currentLevel >= levels.Count;
    public int MaxLevel => Mathf.Max(1, levels?.Count ?? 1);

    public BitMinerLevelStats CurrentStats
    {
        get
        {
            if (levels == null || levels.Count == 0)
                return new BitMinerLevelStats();
            int idx = Mathf.Clamp(currentLevel - 1, 0, levels.Count - 1);
            return levels[idx];
        }
    }

    public BitMinerLevelStats NextStats
    {
        get
        {
            if (IsMaxLevel) return null;
            return levels[currentLevel]; // currentLevel is 1-based; next index is the same number
        }
    }

    public int CurrentCapacity => CurrentStats.capacity;
    public int CurrentProductionPerHour => CurrentStats.productionPerHour;

    // Upgrade requirements for going FROM current level TO next level
    public int NextUpgradeCostBits => NextStats != null ? NextStats.upgradeCostBits : 0;
    public int NextUpgradeCostScrap => NextStats != null ? NextStats.upgradeCostScrap : 0;
    public int NextUpgradeTimeSeconds => NextStats != null ? NextStats.upgradeTimeSeconds : 0;

    // -----------------------------------------------------------------------

    private void Awake()
    {
        buildingName = "Bit Miner";

        // Grab info panel (existing behavior)
        GameObject panel = GameObject.Find("BitMinerInfoPanel");
        if (panel != null)
            infoCanvas = panel.GetComponent<CanvasGroup>();

        // Clamp current level just in case
        if (levels == null || levels.Count == 0)
        {
            levels = new List<BitMinerLevelStats> { new BitMinerLevelStats() };
        }
        currentLevel = Mathf.Clamp(currentLevel, 1, levels.Count);

        // Register with BitManager
        if (BitManager.Instance != null)
        {
            BitManager.Instance.RegisterMiner(this);
        }
    }

    private void OnDestroy()
    {
        if (BitManager.Instance != null)
        {
            BitManager.Instance.UnregisterMiner(this);
        }
    }

    public override void OpenInfoPanel()
    {
        if (infoCanvas != null)
        {
            infoCanvas.alpha = 1f;
            infoCanvas.interactable = true;
            infoCanvas.blocksRaycasts = true;

            BitMinerInfoUI ui = infoCanvas.GetComponent<BitMinerInfoUI>();
            if (ui != null)
            {
                // If your UI currently reads fields directly, you can update it to call these:
                // ui.SetInfo(this) can still work; just have it read Current/Next getters.
                ui.SetInfo(this);
            }
        }
    }

    public void CloseInfoPanel()
    {
        if (infoCanvas != null)
        {
            infoCanvas.alpha = 0f;
            infoCanvas.interactable = false;
            infoCanvas.blocksRaycasts = false;
        }
    }

    // Production tick uses current level's stats
    public void TickProduction(float deltaTime)
    {
        if (isUpgrading) return;

        float ratePerSecond = CurrentProductionPerHour / 3600f;
        currentStored += ratePerSecond * deltaTime;
        currentStored = Mathf.Min(currentStored, CurrentCapacity);
    }

    public void Collect()
    {
        int collected = Mathf.FloorToInt(currentStored);
        if (collected > 0 && BitManager.Instance != null)
        {
            BitManager.Instance.AddBits(collected);
            currentStored -= collected;
        }
    }

    // -- Upgrade flow helpers ------------------------------------------------
    // Leave the actual currency deduction to your Upgrade/Build system.
    // Call CanUpgrade() -> handle costs/time externally -> ApplyUpgrade().

    public bool CanUpgrade()
    {
        return !IsMaxLevel;
    }

    public void ApplyUpgrade()
    {
        if (IsMaxLevel) return;

        // Optional: spillover protection if capacity increases mid-upgrade
        int newCapacity = levels[currentLevel].capacity; // next level capacity
        currentLevel++;
        currentStored = Mathf.Min(currentStored, newCapacity);
    }

    public void BeginUpgradeTimer()
    {
        if (!CanUpgrade()) return;
        isUpgrading = true;
        upgradeTimeRemaining = NextUpgradeTimeSeconds;
        upgradeEndTimestamp = Time.time + upgradeTimeRemaining; // absolute
    }

    private void Update()
    {
        if (!isUpgrading) return;

        // Recompute remaining from the absolute end time (robust against hiccups)
        upgradeTimeRemaining = Mathf.Max(0f, upgradeEndTimestamp - Time.time);

        if (upgradeTimeRemaining <= 0f)
        {
            isUpgrading = false;
            upgradeEndTimestamp = -1f;
            upgradeTimeRemaining = 0f;
            ApplyUpgrade();
        }
    }
}
