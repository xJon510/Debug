using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BitMinerInfoUI : MonoBehaviour
{
    [Header("Title")]
    public TMP_Text titleText;

    [Header("Info Tab")]
    public TMP_Text descriptionText;
    public TMP_Text capacityText;
    public TMP_Text productionText;
    public TMP_Text stateText;
    public Slider capacitySlider;

    [Header("Upgrade Tab")]
    public TMP_Text upgradeCostText;
    public TMP_Text upgradeTimeText;

    private BitMiner trackedMiner;

    public void SetInfo(BitMiner miner)
    {
        trackedMiner = miner;

        titleText.text = miner.buildingName;
        descriptionText.text = miner.description;

        if (capacitySlider != null)
        {
            capacitySlider.maxValue = miner.capacity;
        }

        productionText.text = $"Production: <color=#00FFFF>+{miner.productionPerHour} Bits / Hr</color>";

        upgradeCostText.text = $"Upgrade Cost: <color=#00FFFF>{miner.upgradeCostBits}</color>";
        upgradeTimeText.text = $"Upgrade Time: <color=#00FFFF>{miner.upgradeTime}</color>";
    }

    private void Update()
    {
        if (trackedMiner != null)
        {
            // Update dynamic UI
            capacityText.text = $"{Mathf.FloorToInt(trackedMiner.currentStored)}/{trackedMiner.capacity}";

            if (capacitySlider != null)
            {
                capacitySlider.maxValue = trackedMiner.capacity;
                capacitySlider.value = trackedMiner.currentStored;
            }

            if (trackedMiner.currentStored >= trackedMiner.capacity)
            {
                stateText.text = $"State: <color=#FF0000>STORAGE FULL</color>";
            }
            else
            {
                stateText.text = $"State: <color=#00FFFF>Processing</color>";
            }
        }
    }

    public void CollectButtonPressed()
    {
        if (trackedMiner != null)
        {
            trackedMiner.Collect();
        }
    }
}
