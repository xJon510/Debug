using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerEXP : MonoBehaviour
{
    public static PlayerEXP Instance;

    [Header("Level / EXP")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int currentExp = 0;
    [SerializeField] private int expToNextLevel = 100; // starting requirement
    [SerializeField] private float expGrowthRate = 1.2f; // 20% harder each level
    public float difficultyMultiplier = 1.0f;

    [Header("UI")]
    public Slider expSlider;
    public TMP_Text levelText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        RefreshUI();
    }

    public void AddExp(int amount)
    {
        int adjusted = Mathf.RoundToInt(amount * difficultyMultiplier);
        currentExp += adjusted;

        while (currentExp >= expToNextLevel)
        {
            currentExp -= expToNextLevel;
            LevelUp();
        }

        RefreshUI();
    }

    private void LevelUp()
    {
        currentLevel++;
        expToNextLevel = Mathf.RoundToInt(expToNextLevel * expGrowthRate);

        // Hook into StatManager here:
        StatManager.Instance.ModifyStat(stats =>
        {
            stats.health += 5f;
            stats.damage += 0.5f;
            stats.moveSpeed += 0.1f;
        });

        Debug.Log($"Leveled up to {currentLevel}!");
    }

    private void RefreshUI()
    {
        if (expSlider != null)
        {
            expSlider.maxValue = expToNextLevel;
            expSlider.value = currentExp;
        }

        if (levelText != null)
        {
            levelText.text = $"LVL {currentLevel}";
        }
    }
}
