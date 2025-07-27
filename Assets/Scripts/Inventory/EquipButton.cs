using UnityEngine;

public class EquipButton : MonoBehaviour
{
    public EquipmentItem itemToEquip;

    public void Equip()
    {
        if (itemToEquip != null)
        {
            EquippedGearManager.EquipItem(itemToEquip);

            Debug.Log($"Equipped {itemToEquip.slot} with:");
            foreach (var stat in itemToEquip.rolledStats)
                Debug.Log($"{stat.statName}: +{stat.value}");
        }
    }
}
