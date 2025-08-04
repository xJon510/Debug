using UnityEngine;
using UnityEngine.AI;

public class Tier1Worm : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 10f;
    public float contactDamage = 1f;
    [Range(0.1f, 1f)]
    public float size = 1f; // 1 = normal, 0.5 = mini worm

    private float currentHealth;
    private Transform player;
    private UnityEngine.AI.NavMeshAgent agent;

    private bool playerInRange = false;
    private float damageCooldown = 1f; // Time between damage ticks
    private float damageTimer = 0f;
    private PlayerHealth cachedPlayerHealth;

    void Start()
    {
        ApplySizeScaling();

        currentHealth = maxHealth;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        agent = GetComponent<NavMeshAgent>();

        if (player == null)
        {
            Debug.LogWarning("SwarmEnemy couldn't find player. Make sure Player has the 'Player' tag.");
        }
    }

    void OnEnable()
    {
        currentHealth = maxHealth;
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

    void ApplySizeScaling()
    {
        // Scale health down proportionally
        maxHealth *= size;

        // Scale the model
        transform.localScale = new Vector3(size, size, size);

        // Adjust BoxCollider if mini (hardcoded for 0.5 case)
        BoxCollider col = GetComponent<BoxCollider>();
        if (col != null && Mathf.Approximately(size, 0.5f))
        {
            col.center = new Vector3(0f, 2f, 0f);
            col.size = new Vector3(1.2f, 5f, 1.2f);
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

        EnemyManager.Instance.ReturnEnemy(gameObject);
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
