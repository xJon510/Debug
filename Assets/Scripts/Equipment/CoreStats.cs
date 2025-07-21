using UnityEngine;

public class CoreStats : MonoBehaviour
{
    [Header("Movement")]
    public float health = 100f;
    public float moveSpeed = 5f;
    public float dashSpeed = 10f;
    public float dashLength = 0.2f;
    public float dashCooldown = 1f;

    [Header("Combat")]
    public float fireRate = 1f;
    public float damage = 10f;
    public float critChance = 0.10f;  // 10%
    public float critMultiplier = 2f;  //  2x Damage

    [Header("Projectile")]
    public int bulletCount = 1;
    public int bulletPierce = 0;
    public float bulletRange = 10f;
    public float knockback = 1f;

    [Header("Utility")]
    public float cooldownReduction = 0f;

    // Optional: Singleton for quick access
    public static CoreStats Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
