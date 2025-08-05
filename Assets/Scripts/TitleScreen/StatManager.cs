using System;
using UnityEngine;

[System.Serializable]
public class StatBlock
{
    public float health = 10f;
    public float armor = 0f;
    public float moveSpeed = 10f;
    public float dashSpeed = 40f;
    public float dashLength = 0.2f;
    public float dashCooldown = 5f;

    public float fireRate = 0.5f;
    public float damage = 1f;
    public float critChance = 0.0f;
    public float critMultiplier = 2f;

    public int bulletCount = 1;
    public int bulletPierce = 0;
    public float bulletRange = 10f;  // 0.5 lifespan
    public float knockback = 0f;

    public float cooldownReduction = 0f;
}

public class StatManager : MonoBehaviour
{
    public static StatManager Instance;
    public StatBlock baseStats = new StatBlock();
    public event Action OnStatsChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ModifyStat(Action<StatBlock> change)
    {
        change(baseStats);
        Debug.Log($"[StatManager] Stats modified -> Health: {baseStats.health}, Damage: {baseStats.damage}, MoveSpeed: {baseStats.moveSpeed}");
        OnStatsChanged?.Invoke();
        Debug.Log("[StatManager] OnStatsChanged invoked");
    }
}
