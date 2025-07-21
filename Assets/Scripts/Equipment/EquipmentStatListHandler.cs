using UnityEngine;
using TMPro;

public class EquipmentStatListHandler : MonoBehaviour
{
    [Header("UI Text References (Match Stat Order)")]
    public TMP_Text healthText;
    public TMP_Text moveSpeedText;
    public TMP_Text dashSpeedText;
    public TMP_Text dashLengthText;
    public TMP_Text dashCooldownText;

    public TMP_Text fireRateText;
    public TMP_Text damageText;
    public TMP_Text critChanceText;
    public TMP_Text critMultiplierText;

    public TMP_Text bulletCountText;
    public TMP_Text bulletPierceText;
    public TMP_Text bulletRangeText;
    public TMP_Text knockbackText;

    public TMP_Text cooldownReductionText;

    void Start()
    {
        UpdateStatUI();
    }

    public void UpdateStatUI()
    {
        var stats = StatManager.Instance.baseStats;

        healthText.text = $"Health: {stats.health}";
        moveSpeedText.text = $"Move Speed: {stats.moveSpeed}";
        dashSpeedText.text = $"Dash Speed: {stats.dashSpeed}";
        dashLengthText.text = $"Dash Length: {stats.dashLength}s";
        dashCooldownText.text = $"Dash Cooldown: {stats.dashCooldown}s";

        fireRateText.text = $"Fire Rate: {stats.fireRate}";
        damageText.text = $"Damage: {stats.damage}";
        critChanceText.text = $"Crit Chance: {stats.critChance * 100f}%";
        critMultiplierText.text = $"Crit Multiplier: {stats.critMultiplier}x";

        bulletCountText.text = $"Bullet Count: {stats.bulletCount}";
        bulletPierceText.text = $"Bullet Pierce: {stats.bulletPierce}";
        bulletRangeText.text = $"Bullet Range: {stats.bulletRange}";
        knockbackText.text = $"Knockback: {stats.knockback}";

        cooldownReductionText.text = $"Cooldown Reduction: {stats.cooldownReduction * 100f}%";
    }
}
