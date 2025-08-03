using UnityEngine;
using UnityEngine.AI;

public class SwarmEnemy : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 10f;
    public float contactDamage = 1f;

    private float currentHealth;
    private Transform player;
    private UnityEngine.AI.NavMeshAgent agent;

    private bool playerInRange = false;
    private float damageCooldown = 1f; // Time between damage ticks
    private float damageTimer = 0f;
    private PlayerHealth cachedPlayerHealth;

    void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        agent = GetComponent<NavMeshAgent>();

        if (player == null)
        {
            Debug.LogWarning("SwarmEnemy couldn't find player. Make sure Player has the 'Player' tag.");
        }
    }

    void Update()
    {
        if (player != null && agent != null)
        {
            agent.SetDestination(player.position);
        }

        if (playerInRange && cachedPlayerHealth != null)
        {
            damageTimer -= Time.deltaTime;

            if (damageTimer <= 0f)
            {
                cachedPlayerHealth.TakeDamage(contactDamage);
                Debug.Log($"Enemy damaged player for {contactDamage} (sustained) {cachedPlayerHealth.currentHealth}");

                damageTimer = damageCooldown;
            }
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        // Placeholder: just destroy for now
        GetComponent<ExpReward>()?.GrantExp();

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            cachedPlayerHealth = other.GetComponent<PlayerHealth>();
            if (cachedPlayerHealth != null)
            {
                cachedPlayerHealth.TakeDamage(contactDamage); // initial hit
                damageTimer = damageCooldown; // reset timer after instant hit
                playerInRange = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            cachedPlayerHealth = null;
        }
    }
}
