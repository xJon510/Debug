using System;
using UnityEngine;

public class StatHandler : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private Movement movement;
    private FireBullet fireBullet;

    void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        movement = GetComponent<Movement>();
        fireBullet = GetComponent<FireBullet>();
    }

    void Start()
    {
        StatManager.Instance.OnStatsChanged += ApplyStats;
    }

    public void ApplyStats()
    {
        var stats = StatManager.Instance.baseStats;
        Debug.Log($"[StatHandler] ApplyStats called -> Health: {stats.health}, Damage: {stats.damage}, MoveSpeed: {stats.moveSpeed}");

        if (playerHealth != null)
        {
            Debug.Log($"[StatHandler] Applying to PlayerHealth (before) Max: {playerHealth.maxHealth}, Current: {playerHealth.currentHealth}");
            float diff = stats.health - playerHealth.maxHealth;
            playerHealth.maxHealth = stats.health;
            playerHealth.currentHealth += diff;
            playerHealth.armor = stats.armor;
            playerHealth.RefreshUI(); // Add this helper in PlayerHealth below
            Debug.Log($"[StatHandler] PlayerHealth updated (after) Max: {playerHealth.maxHealth}, Current: {playerHealth.currentHealth}");
        }

        if (movement != null)
        {
            movement.moveSpeed = stats.moveSpeed;
            movement.dashSpeed = stats.dashSpeed;
            movement.dashDuration = stats.dashLength;
            movement.dashCooldown = stats.dashCooldown;
            Debug.Log($"[StatHandler] Movement updated -> MoveSpeed: {movement.moveSpeed}, DashSpeed: {movement.dashSpeed}");
        }

        if (fireBullet != null)
        {
            fireBullet.fireRate = stats.fireRate;
            fireBullet.bulletCount = stats.bulletCount;
            Debug.Log($"[StatHandler] FireBullet updated -> FireRate: {fireBullet.fireRate}, BulletCount: {fireBullet.bulletCount}");
        }

        // If you want to apply bullet pierce, knockback, damage, etc.
        // Those are already being applied when the bullet is pooled via Bullet.cs
    }
}
