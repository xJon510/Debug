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
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Placeholder damage logic
            Debug.Log($"Enemy damaged player for {contactDamage} (placeholder)");

            // Optionally destroy on contact
            // Destroy(gameObject);
        }
    }
}
