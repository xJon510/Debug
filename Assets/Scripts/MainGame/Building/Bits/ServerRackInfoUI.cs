using UnityEngine;
using TMPro;

public class ServerRackInfoUI : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public TMP_Text capacityText;
    public TMP_Text upgradeCostText;
    public TMP_Text upgradeTimeText;

    public void SetInfo(ServerRack rack)
    {
        titleText.text = rack.buildingName;
        descriptionText.text = rack.description;
        capacityText.text = $"Capacity: <color=#00FFFF>{rack.capacity}</color>";
        upgradeCostText.text = $"Upgrade Cost: <color=#00FFFF>{rack.upgradeCost}</color>";
        upgradeTimeText.text = $"Upgrade Time: <color=#00FFFF>{rack.upgradeTime}</color>";
    }
}
