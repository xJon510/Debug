using UnityEngine;
using UnityEngine.InputSystem;

public class FireBullet : MonoBehaviour
{
    [Header("References")]
    public GameObject bulletPrefab;
    public Transform barrelTransform;

    [Header("Firing")]
    public float fireRate = 0.1f; // Seconds between bullets
    public float bulletCount = 1;

    [Header("Spread Type")]
    public bool useAngledSpread = false; // false = linear burst, true = angled spread

    private float fireTimer = 0f;
    private bool isFiring = false;

    private void Awake()
    {
        fireRate = StatManager.Instance.baseStats.fireRate;
        bulletCount = StatManager.Instance.baseStats.bulletCount;
    }

    private void Update()
    {
        if (isFiring)
        {
            fireTimer += Time.deltaTime;
            if (fireTimer >= fireRate)
            {
                Fire();
                fireTimer = 0f;
            }
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        isFiring = context.ReadValue<float>() > 0f;
        if (context.started)
        {
            fireTimer = fireRate; // Instant fire on press
        }
    }

    private void Fire()
    {
        int count = Mathf.Max(1, Mathf.RoundToInt(bulletCount));
        float offsetAmount = 0.5f;        // Linear spacing
        float totalSpreadAngle = 25f;     // Angular spread

        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos = barrelTransform.position;
            Quaternion rotation = barrelTransform.rotation;

            if (!useAngledSpread)
            {
                // Linear Tight Burst
                float centerOffset = offsetAmount * (count - 1) / 2f;
                Vector3 offset = barrelTransform.right * (offsetAmount * i - centerOffset);
                spawnPos += offset;
            }
            else
            {
                // Angled Spread
                if (count > 1)
                {
                    float angleStep = totalSpreadAngle / (count - 1);
                    float startAngle = -totalSpreadAngle / 2f;
                    float angle = startAngle + i * angleStep;
                    rotation = Quaternion.Euler(0, barrelTransform.eulerAngles.y + angle, 0);
                }
            }

            GameObject bullet = BulletPoolManager.Instance.GetBullet(spawnPos, rotation);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.sourcePlayer = this.transform;
            }
        }
    }
}
