using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifespan = 2f;

    private float timer;

    void OnEnable()
    {
        timer = 0f;
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
        // Check for ground or other things we want to collide with
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (other.TryGetComponent(out SwarmEnemy enemy))
            {
                enemy.TakeDamage(1f); // Damage can be tweaked later
            }

            HandleImpact(other);
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
