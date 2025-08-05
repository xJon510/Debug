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


    private List<GameObject> pooledEnemies = new List<GameObject>();
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
            yield return StartCoroutine(PreparePool(config.enemyPrefab, config.enemyCount));

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
        int toAdd = neededCount - CountAvailable(prefab);
        for (int i = 0; i < toAdd; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pooledEnemies.Add(obj);
            yield return null; // spread pooling across frames
        }
    }

    private int CountAvailable(GameObject prefab)
    {
        int count = 0;
        foreach (var e in pooledEnemies)
        {
            if (!e.activeSelf && e.name.Contains(prefab.name))
                count++;
        }
        return count;
    }

    public GameObject GetEnemy(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        foreach (var e in pooledEnemies)
        {
            if (!e.activeSelf && e.name.Contains(prefab.name))
            {
                e.transform.SetPositionAndRotation(pos, rot);
                e.SetActive(true);
                activeEnemies++;
                return e;
            }
        }

        // Fallback (shouldn’t hit if PreparePool ran correctly)
        GameObject obj = Instantiate(prefab, pos, rot);
        pooledEnemies.Add(obj);
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
}
