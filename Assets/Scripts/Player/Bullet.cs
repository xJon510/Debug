using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifespan = 2f;
    public float damage = 1f;
    public float critChance = 0f;
    public float critMultiplier = 2f;
    public float knockback = 0f;

    public int bulletPierce = 0; // How many enemies it can pierce through

    public GameObject damagePopupPrefab;
    public Transform popupCanvasParent;

    private int enemiesHit = 0;
    private float timer;
    [HideInInspector] public Transform sourcePlayer;

    void OnEnable()
    {
        timer = 0f;
        enemiesHit = 0; // reset when reused from pool
        damage = StatManager.Instance.baseStats.damage;
        knockback = StatManager.Instance.baseStats.knockback;
        lifespan = StatManager.Instance.baseStats.bulletRange / 20;
        critChance = StatManager.Instance.baseStats.critChance;
        critMultiplier = StatManager.Instance.baseStats.critMultiplier;
    }

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        timer += Time.deltaTime;
        if (timer >= lifespan)
        {
            BulletPoolManager.Instance.ReturnBullet(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.layer == LayerMask.NameToLayer("Buildings"))
        {
            HandleImpact(other); // Instantly return to pool
            return;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (other.TryGetComponent(out SwarmEnemy enemy))
            {
                int critTier;
                float finalDamage = CalculateCritDamage(out critTier);
                enemy.TakeDamage(finalDamage);

                if (damagePopupPrefab != null)
                {
                    Vector3 popupPos = enemy.transform.position + Vector3.up * 2f;

                    // Force local Z position to 0 relative to the canvas
                    GameObject popup = Instantiate(damagePopupPrefab, popupCanvasParent);
                    popup.transform.position = popupPos;
                    popup.transform.rotation = Quaternion.Euler(75f, 0f, 0f);

                    // Optional: Lock Z in local space
                    Vector3 local = popup.transform.localPosition;
                    local.z = 0f;
                    popup.transform.localPosition = local;

                    Color critColor = Color.white;

                    switch (critTier)
                    {
                        case 0: critColor = Color.white; break;
                        case 1: critColor = Color.yellow; break;
                        case 2: critColor = new Color(1f, 0.5f, 0f); break; // orange
                        case 3: critColor = Color.red; break;
                        case 4: critColor = new Color(0.7f, 0f, 1f); break; // purple
                        default: critColor = Color.pink; break; // mega crit
                    }

                    popup.GetComponent<DamagePopup>().Setup(finalDamage, critColor);
                }

                Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
                if (enemyRb != null && sourcePlayer != null)
                {
                    Vector3 knockbackDir = (enemy.transform.position - sourcePlayer.position);
                    knockbackDir.y = 0f;
                    knockbackDir.Normalize();
                    enemyRb.AddForce(knockbackDir * knockback, ForceMode.Impulse);
                }
            }

            enemiesHit++;
            if (enemiesHit > bulletPierce)
            {
                HandleImpact(other);
            }
        }
    }

    private void HandleImpact(Collider hit)
    {
        // Placeholder for visual effects
        // You can call a particle system or spawn an object here
        // For now, just return to pool
        Debug.Log($"Bullet hit {hit.name}");

        // (Optional) spawn VFX/mark here

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        BulletPoolManager.Instance.ReturnBullet(gameObject);
    }

    private float CalculateCritDamage(out int critTier)
    {
        int guaranteedCrits = Mathf.FloorToInt(critChance / 100f);
        float extraCritChance = (critChance % 100f) / 100f;

        // RNG for extra tier
        if (Random.value < extraCritChance)
        {
            guaranteedCrits++;
        }

        critTier = guaranteedCrits;

        // Multiply base damage with crit tier scaling
        float finalDamage = damage * (1f + guaranteedCrits * (critMultiplier - 1f));
        return finalDamage;
    }
}
