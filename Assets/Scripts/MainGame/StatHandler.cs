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

        if (playerHealth != null)
        {
            playerHealth.maxHealth = stats.health;
            playerHealth.RefreshUI(); // Add this helper in PlayerHealth below
            playerHealth.armor = stats.armor;
        }

        if (movement != null)
        {
            movement.moveSpeed = stats.moveSpeed;
            movement.dashSpeed = stats.dashSpeed;
            movement.dashDuration = stats.dashLength;
            movement.dashCooldown = stats.dashCooldown;
        }

        if (fireBullet != null)
        {
            fireBullet.fireRate = stats.fireRate;
            fireBullet.bulletCount = stats.bulletCount;
        }

        // If you want to apply bullet pierce, knockback, damage, etc.
        // Those are already being applied when the bullet is pooled via Bullet.cs
    }
}
