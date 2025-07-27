// =====================
// EQUIPMENT ITEM CLASS
// =====================
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(menuName = "Equipment/Create New Equipment Item")]
public class EquipmentItem : ScriptableObject
{
    public EquipmentSlot slot;
    public int tier;
    public List<RolledStat> rolledStats = new();

    public EquipmentItem(EquipmentSlot slot, int tier)
    {
        this.slot = slot;
        this.tier = tier;
        RollStats();
    }

    private void RollStats()
    {
        rolledStats.Clear();
        var validStats = EquipmentStatRules.GetStatPoolForSlot(slot);
        int statCount = EquipmentStatRules.GetStatCountForSlotAndTier(slot, tier);
        var shuffledStats = validStats.OrderBy(s => UnityEngine.Random.value).ToList();

        for (int i = 0; i < statCount && i < shuffledStats.Count; i++)
        {
            string stat = shuffledStats[i];
            Vector2 range = EquipmentStatRules.GetValueRange(stat, tier);
            float rolledValue = Mathf.Round(UnityEngine.Random.Range(range.x, range.y) * 100f) / 100f;

            rolledStats.Add(new RolledStat
            {
                statName = stat,
                value = rolledValue
            });
        }
    }

    public void Equip()
    {
        StatManager.Instance.ModifyStat(stats =>
        {
            foreach (var stat in rolledStats)
            {
                ApplyStat(ref stats, stat.statName, stat.value);
            }
        });
    }

    public void Unequip()
    {
        StatManager.Instance.ModifyStat(stats =>
        {
            foreach (var stat in rolledStats)
            {
                ApplyStat(ref stats, stat.statName, -stat.value);
            }
        });
    }

    private void ApplyStat(ref StatBlock stats, string statName, float value)
    {
        switch (statName)
        {
            case "Health": stats.health += value; break;
            case "MovementSpeed": stats.moveSpeed += value; break;
            case "DashSpeed": stats.dashSpeed += value; break;
            case "DashLength": stats.dashLength += value; break;
            case "FireRate": stats.fireRate += value; break;
            case "Damage": stats.damage += value; break;
            case "CritChance": stats.critChance += value; break;
            case "CritMultiplier": stats.critMultiplier += value; break;
            case "BulletCount": stats.bulletCount += Mathf.RoundToInt(value); break;
            case "BulletPierce": stats.bulletPierce += Mathf.RoundToInt(value); break;
            case "BulletRange": stats.bulletRange += value; break;
            case "Knockback": stats.knockback += value; break;
            case "CooldownReduction": stats.cooldownReduction += value; break;
            case "DashCooldown": stats.dashCooldown += value; break;
            default: Debug.LogWarning("Unknown stat: " + statName); break;
        }
    }
}