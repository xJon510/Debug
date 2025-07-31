using UnityEngine;
using TMPro;

public class BitMinerInfoUI : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public TMP_Text capacityText;
    public TMP_Text productionText;
    public TMP_Text upgradeCostText;
    public TMP_Text upgradeTimeText;

    public void SetInfo(BitMiner miner)
    {
        titleText.text = miner.buildingName;
        descriptionText.text = miner.description;
        capacityText.text = $"Capacity: <color=#00FFFF>{miner.capacity}</color>";
        productionText.text = $"Production: <color=#00FFFF>{miner.productionPerHour} Bits/Hr</color>";
        upgradeCostText.text = $"Upgrade Cost: <color=#00FFFF>{miner.upgradeCost}</color>";
        upgradeTimeText.text = $"Upgrade Time: <color=#00FFFF>{miner.upgradeTime}</color>";
    }
}
