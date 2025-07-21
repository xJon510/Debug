using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifespan = 2f;
    public float damage = 1f;
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
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            HandleImpact(other); // Instantly return to pool
            return;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (other.TryGetComponent(out SwarmEnemy enemy))
            {
                enemy.TakeDamage(damage);

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

                    popup.GetComponent<DamagePopup>().Setup(damage);
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
}
