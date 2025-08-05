using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float armor = 0f;
    public float currentHealth;

    [Header("UI")]
    public Slider healthSlider;
    public TMP_Text healthText;
    public TMP_Text healthTextHolder;
    public TMP_Text healthTextVisible;

    void Start()
    {
        currentHealth = StatManager.Instance.baseStats.health;
        maxHealth = StatManager.Instance.baseStats.health;
        armor = StatManager.Instance.baseStats.armor;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"{currentHealth}/{maxHealth}";
            healthTextHolder.text = $"{currentHealth}/{maxHealth}";
            healthTextVisible.text = $"{currentHealth}/{maxHealth}";
        }

    }

    public void TakeDamage(float amount)
    {
        float armorTotal = Mathf.Clamp(armor, 0f, 80f);         // Cap Armor at 80%
        float damageTaken = amount * (1f - armor / 100f);

        currentHealth -= damageTaken;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
            healthText.text = $"{currentHealth}/{maxHealth}";
            healthTextHolder.text = $"{currentHealth}/{maxHealth}";
            healthTextVisible.text = $"{currentHealth}/{maxHealth}";
        }

        Debug.Log($"Player took {damageTaken} damage! Current HP: {currentHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player has died!");
        // Add death logic here (disable movement, show game over screen, etc.)
    }

    public void RefreshUI()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"{currentHealth}/{maxHealth}";
            healthTextHolder.text = $"{currentHealth}/{maxHealth}";
            healthTextVisible.text = $"{currentHealth}/{maxHealth}";
        }
    }
}
