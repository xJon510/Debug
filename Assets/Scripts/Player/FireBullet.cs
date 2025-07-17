using UnityEngine;
using UnityEngine.InputSystem;

public class FireBullet : MonoBehaviour
{
    [Header("References")]
    public GameObject bulletPrefab;
    public Transform barrelTransform;

    [Header("Firing")]
    public float fireRate = 0.1f; // Seconds between bullets
    private float fireTimer = 0f;
    private bool isFiring = false;

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
        if (bulletPrefab != null && barrelTransform != null)
        {
            GameObject bullet = Instantiate(
                bulletPrefab,
                barrelTransform.position,
                transform.rotation // use player’s current facing direction
            );
        }
    }
}
