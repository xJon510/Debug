using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    public TMP_Text waveCountText;
    public TMP_Text waveCountHolder;
    public TMP_Text waveCountVisible;

    public EnemyCategory enemyCategory;

    public Transform[] spawnPoints;          // your 4 platforms
    public List<WaveConfig> waves;           // assign waves in inspector
    public int currentWave = 0;
    private int roundRobinIndex = 0;

    public IEnumerator SpawnWaveCoroutine(WaveConfig config)
    {
        waveCountText.text = $"Wave {currentWave + 1}";
        waveCountHolder.text = $"Wave {currentWave + 1}";
        waveCountVisible.text = $"Wave {currentWave + 1}";

        switch (config.spawnMode)
        {
            case SpawnMode.Immediate:
                SpawnEnemies(config.category, config.enemyCount, config);
                break;
            case SpawnMode.Staggered:
                for (int i = 0; i < config.enemyCount; i++)
                {
                    SpawnEnemies(config.category, 1, config);
                    yield return new WaitForSeconds(config.spawnRate);
                }
                break;
            case SpawnMode.Burst:
                int spawned = 0;
                while (spawned < config.enemyCount)
                {
                    int group = Mathf.Min(config.burstSize, config.enemyCount - spawned);
                    SpawnEnemies(config.category, group, config);
                    spawned += group;
                    yield return new WaitForSeconds(config.spawnRate);
                }
                break;
        }

        currentWave++;
    }

    private void SpawnEnemies(EnemyCategory category, int count, WaveConfig config)
    {
        GameObject prefab = EnemyManager.Instance.GetPrefabForCategory(category);
        if (prefab == null) return;

        for (int i = 0; i < count; i++)
        {
            Transform spawnPoint = ChooseSpawnPoint(config);
            EnemyManager.Instance.GetEnemy(prefab, spawnPoint.position, Quaternion.identity);
        }
    }

    private Transform ChooseSpawnPoint(WaveConfig config)
    {
        switch (config.distribution)
        {
            case SpawnDistribution.RoundRobin:
                roundRobinIndex = (roundRobinIndex + 1) % spawnPoints.Length;
                return spawnPoints[roundRobinIndex];

            case SpawnDistribution.Random:
                return spawnPoints[Random.Range(0, spawnPoints.Length)];

            case SpawnDistribution.SingleSide:
                return spawnPoints[0]; // pick one, or random one-time at wave start

            case SpawnDistribution.Weighted:
                return WeightedChoice(config.spawnWeights);

            case SpawnDistribution.Even:
            default:
                // just spread evenly by index
                return spawnPoints[Random.Range(0, spawnPoints.Length)];
        }
    }

    private Transform WeightedChoice(float[] weights)
    {
        float total = 0f;
        foreach (float w in weights) total += w;
        float r = Random.Range(0f, total);

        float sum = 0f;
        for (int i = 0; i < weights.Length; i++)
        {
            sum += weights[i];
            if (r <= sum)
                return spawnPoints[i];
        }
        return spawnPoints[0]; // fallback
    }
}
