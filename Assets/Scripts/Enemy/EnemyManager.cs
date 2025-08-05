using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance; // quick singleton
    public WaveManager waveManager;
    public StartWaveButton startWaveButton;

    public TMP_Text enemyCountText;
    public TMP_Text enemyCountHolder;
    public TMP_Text enemyCountVisible;

    [Header("Enemy Prefabs")]
    public GameObject babyWormPrefab;
    public GameObject wormTier1Prefab;
    public GameObject trojanPrefab;
    public GameObject spywarePrefab;
    public GameObject ransomwarePrefab;

    private Dictionary<GameObject, List<GameObject>> enemyPools = new Dictionary<GameObject, List<GameObject>>();
    private int activeEnemies = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Called by UI button to start first wave
    public void BeginWaves()
    {
        StartCoroutine(RunWaves());

        WaveConfig config = waveManager.waves[0];

        enemyCountText.text = $"{config.enemyCount} REMAINING";
        enemyCountHolder.text = $"{config.enemyCount} REMAINING";
        enemyCountVisible.text = $"{config.enemyCount} REMAINING";
    }

    private IEnumerator RunWaves()
    {
        for (int i = 0; i < waveManager.waves.Count; i++)
        {
            WaveConfig config = waveManager.waves[i];

            // Prep delay between waves
            yield return new WaitForSeconds(2f);

            // Ensure enough pooled enemies exist
            GameObject prefab = GetPrefabForCategory(config.category);
            if (prefab != null)
            {
                yield return StartCoroutine(PreparePool(prefab, config.enemyCount));
            }

            enemyCountText.text = $"{config.enemyCount} REMAINING";
            enemyCountHolder.text = $"{config.enemyCount} REMAINING";
            enemyCountVisible.text = $"{config.enemyCount} REMAINING";

            // Spawn wave
            yield return StartCoroutine(waveManager.SpawnWaveCoroutine(config));

            // Wait until all enemies are returned to pool
            yield return new WaitUntil(() => activeEnemies == 0);
        }

        Debug.Log("All waves completed!");

        startWaveButton?.EndWave();
    }

    private IEnumerator PreparePool(GameObject prefab, int neededCount)
    {
        if (!enemyPools.ContainsKey(prefab))
            enemyPools[prefab] = new List<GameObject>();

        List<GameObject> pool = enemyPools[prefab];
        int toAdd = neededCount - CountAvailable(prefab);

        for (int i = 0; i < toAdd; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Add(obj);
            yield return null;
        }

        // Only prepare baby worms if this is a Tier1 worm
        if (prefab == wormTier1Prefab && babyWormPrefab != null)
        {
            if (!enemyPools.ContainsKey(babyWormPrefab))
                enemyPools[babyWormPrefab] = new List<GameObject>();

            List<GameObject> babyPool = enemyPools[babyWormPrefab];
            int babyNeeded = neededCount * 2;
            int babyToAdd = babyNeeded - CountAvailable(babyWormPrefab);

            for (int i = 0; i < babyToAdd; i++)
            {
                GameObject baby = Instantiate(babyWormPrefab);
                baby.SetActive(false);
                babyPool.Add(baby);
                yield return null;
            }
        }
    }

    private int CountAvailable(GameObject prefab)
    {
        if (!enemyPools.ContainsKey(prefab)) return 0;

        int count = 0;
        foreach (var e in enemyPools[prefab])
        {
            if (!e.activeSelf)
                count++;
        }
        return count;
    }

    public GameObject GetEnemy(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        if (!enemyPools.ContainsKey(prefab))
            enemyPools[prefab] = new List<GameObject>();

        foreach (var e in enemyPools[prefab])
        {
            if (!e.activeSelf)
            {
                e.transform.SetPositionAndRotation(pos, rot);
                e.SetActive(true);
                activeEnemies++;
                return e;
            }
        }

        // fallback
        GameObject obj = Instantiate(prefab, pos, rot);
        obj.SetActive(true);
        enemyPools[prefab].Add(obj);
        activeEnemies++;
        return obj;
    }

    public void ReturnEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
        activeEnemies--;
        enemyCountText.text = $"{activeEnemies} REMAINING";
        enemyCountHolder.text = $"{activeEnemies} REMAINING";
        enemyCountVisible.text = $"{activeEnemies} REMAINING";
    }

    public GameObject GetPrefabForCategory(EnemyCategory category)
    {
        switch (category)
        {
            case EnemyCategory.Worms: return wormTier1Prefab;
            case EnemyCategory.Trojans: return trojanPrefab;
            case EnemyCategory.Spyware: return spywarePrefab;
            case EnemyCategory.Ransomware: return ransomwarePrefab;
            default: return null;
        }
    }
}
