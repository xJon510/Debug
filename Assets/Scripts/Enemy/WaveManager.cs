using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    public Transform[] spawnPoints;          // your 4 platforms
    public List<WaveConfig> waves;           // assign waves in inspector
    private int currentWave = 0;
    private int roundRobinIndex = 0;

    public void StartNextWave()
    {
        if (currentWave < waves.Count)
        {
            StartCoroutine(SpawnWave(waves[currentWave]));
            currentWave++;
        }
        else
        {
            Debug.Log("All waves completed!");
        }
    }

    private IEnumerator SpawnWave(WaveConfig config)
    {
        switch (config.spawnMode)
        {
            case SpawnMode.Immediate:
                SpawnEnemies(config.enemyPrefab, config.enemyCount, config);
                break;

            case SpawnMode.Staggered:
                for (int i = 0; i < config.enemyCount; i++)
                {
                    SpawnEnemies(config.enemyPrefab, 1, config);
                    yield return new WaitForSeconds(config.spawnRate);
                }
                break;

            case SpawnMode.Burst:
                int spawned = 0;
                while (spawned < config.enemyCount)
                {
                    int group = Mathf.Min(config.burstSize, config.enemyCount - spawned);
                    SpawnEnemies(config.enemyPrefab, group, config);
                    spawned += group;
                    yield return new WaitForSeconds(config.spawnRate);
                }
                break;
        }
    }

    public IEnumerator SpawnWaveCoroutine(WaveConfig config)
    {
        switch (config.spawnMode)
        {
            case SpawnMode.Immediate:
                SpawnEnemies(config.enemyPrefab, config.enemyCount, config);
                break;

            case SpawnMode.Staggered:
                for (int i = 0; i < config.enemyCount; i++)
                {
                    SpawnEnemies(config.enemyPrefab, 1, config);
                    yield return new WaitForSeconds(config.spawnRate);
                }
                break;

            case SpawnMode.Burst:
                int spawned = 0;
                while (spawned < config.enemyCount)
                {
                    int group = Mathf.Min(config.burstSize, config.enemyCount - spawned);
                    SpawnEnemies(config.enemyPrefab, group, config);
                    spawned += group;
                    yield return new WaitForSeconds(config.spawnRate);
                }
                break;
        }
    }

    private void SpawnEnemies(GameObject prefab, int count, WaveConfig config)
    {
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
