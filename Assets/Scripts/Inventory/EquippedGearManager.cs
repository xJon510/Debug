using System.Collections.Generic;
using UnityEngine;

public static class EquippedGearManager
{
    private static Dictionary<EquipmentSlot, EquipmentItem> equippedItems = new();

    public static void EquipItem(EquipmentItem newItem)
    {
        if (equippedItems.TryGetValue(newItem.slot, out var existingItem))
        {
            existingItem.Unequip();
            Debug.Log($"Unequipped previous {existingItem.slot}");
        }

        newItem.Equip();
        EquipmentStatListHandler.Instance?.UpdateStatUI();      // Update UI
        equippedItems[newItem.slot] = newItem;
        Debug.Log($"Equipped new {newItem.slot}");
    }

    public static EquipmentItem GetEquipped(EquipmentSlot slot)
    {
        equippedItems.TryGetValue(slot, out var item);
        return item;
    }
}
