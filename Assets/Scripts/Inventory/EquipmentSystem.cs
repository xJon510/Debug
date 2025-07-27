// =====================
// EQUIPMENT SLOT TYPES
// =====================
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum EquipmentSlot
{
    Helmet,
    Chestpiece,
    Pants,
    Boots,
    Gloves,
    Weapon,
    Ring1,
    Ring2
}

// =====================
// EQUIPMENT STAT ENTRY
// =====================
[System.Serializable]
public class RolledStat
{
    public string statName;
    public float value;
}

// =====================
// STATIC ROLL TABLE LOGIC
// =====================
public static class EquipmentStatRules
{
    private static Dictionary<EquipmentSlot, List<string>> slotStatPool = new() {
        { EquipmentSlot.Pants, new List<string> { "MovementSpeed", "DashSpeed", "DashLength" } },
        // Add more equipment slot pools here
    };

    private static Dictionary<EquipmentSlot, int[]> slotTierStatCounts = new() {
        { EquipmentSlot.Pants, new int[] { 1, 1, 2, 2, 3, 3 } },
        // Define for other slots later
    };

    private static Dictionary<string, List<Vector2>> statValueRanges = new() {
        {
            "MovementSpeed", new List<Vector2> {
                new(0f, 2f), new(2f, 5f), new(5f, 10f), new(10f, 15f), new(15f, 20f), new(20f, 30f)
            }
        },
        {
            "DashSpeed", new List<Vector2> {
                new(0f, 2f), new(2f, 6f), new(6f, 12f), new(12f, 18f), new(18f, 26f), new(26f, 36f)
            }
        },
        {
            "DashLength", new List<Vector2> {
                new(0f, 0.2f), new(0.2f, 0.35f), new(0.35f, 0.55f), new(0.55f, 0.9f), new(0.9f, 1.2f), new(1.2f, 1.7f)
            }
        },
        // Add other stats here
    };

    public static List<string> GetStatPoolForSlot(EquipmentSlot slot)
    {
        return slotStatPool.ContainsKey(slot) ? slotStatPool[slot] : new List<string>();
    }

    public static int GetStatCountForSlotAndTier(EquipmentSlot slot, int tier)
    {
        if (!slotTierStatCounts.ContainsKey(slot)) return 0;
        int[] counts = slotTierStatCounts[slot];
        return Mathf.Clamp(tier - 1, 0, counts.Length - 1) >= 0 ? counts[Mathf.Clamp(tier - 1, 0, counts.Length - 1)] : 0;
    }

    public static Vector2 GetValueRange(string statName, int tier)
    {
        if (!statValueRanges.ContainsKey(statName)) return Vector2.zero;
        var ranges = statValueRanges[statName];
        return ranges[Mathf.Clamp(tier - 1, 0, ranges.Count - 1)];
    }
}
