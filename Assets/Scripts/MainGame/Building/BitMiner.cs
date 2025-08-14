using UnityEngine;

public class BitMiner : Building
{
    [Header("Stats")]
    public int capacity = 300;
    public int productionPerHour = 90;
    public int upgradeCostBits = 200;
    public int upgradeCostScrap = 10;
    public int upgradeTime = 5;
    [TextArea]
    public string description = "An automated process that digs into the digital substrate, extracting stray \"bits\" from memory leaks. Essential for fueling your defenses against the Bugs.";

    [Header("Production Tracking")]
    public float currentStored = 0f;

    private CanvasGroup infoCanvas;

    private void Awake()
    {
        buildingName = "Bit Miner";

        GameObject panel = GameObject.Find("BitMinerInfoPanel");
        if (panel != null)
            infoCanvas = panel.GetComponent<CanvasGroup>();

        if (BitManager.Instance != null)
        {
            BitManager.Instance.RegisterMiner(this);
        }
    }

    private void OnDestroy()
    {
        // Unregister when destroyed/removed
        if (BitManager.Instance != null)
        {
            BitManager.Instance.UnregisterMiner(this);
        }
    }

    public override void OpenInfoPanel()
    {
        if (infoCanvas != null)
        {
            // Enable visibility + interactivity
            infoCanvas.alpha = 1f;
            infoCanvas.interactable = true;
            infoCanvas.blocksRaycasts = true;

            BitMinerInfoUI ui = infoCanvas.GetComponent<BitMinerInfoUI>();
            if (ui != null)
                ui.SetInfo(this);
        }
    }

    public void CloseInfoPanel()
    {
        if (infoCanvas != null)
        {
            // Hide but keep object active
            infoCanvas.alpha = 0f;
            infoCanvas.interactable = false;
            infoCanvas.blocksRaycasts = false;
        }
    }

    // Called every frame (or fixed interval) from BitManager
    public void TickProduction(float deltaTime)
    {
        float ratePerSecond = productionPerHour / 3600f;
        currentStored += ratePerSecond * deltaTime;
        currentStored = Mathf.Min(currentStored, capacity);
    }

    public void Collect()
    {
        int collected = Mathf.FloorToInt(currentStored);
        if (collected > 0)
        {
            BitManager.Instance.AddBits(collected);
            currentStored -= collected;
        }
    }
}
