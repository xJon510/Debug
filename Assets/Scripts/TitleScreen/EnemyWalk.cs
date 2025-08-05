using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWalk : MonoBehaviour
{
    [Header("Enemy Settings")]
    public List<GameObject> enemyPrefabs;  // Your stripped-down enemy prefabs
    public Transform spawnPoint;           // Where enemies appear
    public Transform endPoint;             // The "EndCube" outside camera view
    public float walkSpeed = 2f;           // Speed across the screen
    public float fadeDuration = 1f;        // Fade in time
    public float spawnTimer = 5f;

    private List<GameObject> activeEnemies = new List<GameObject>();

    private void Start()
    {
        // Start looping enemy spawns
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (enemyPrefabs.Count > 0)
            {
                // Pick random enemy prefab
                GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

                // Spawn it at spawnPoint
                Quaternion spawnRot = Quaternion.Euler(0f, -234f, 0f);
                GameObject enemy = Instantiate(prefab, spawnPoint.position, spawnRot);
                activeEnemies.Add(enemy);

                // Fade in
                StartCoroutine(FadeIn(enemy));
            }

            // Delay between spawns (adjust as needed)
            yield return new WaitForSeconds(spawnTimer);
        }
    }

    private IEnumerator FadeIn(GameObject enemy)
    {
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = enemy.transform.localScale;
        enemy.transform.localScale = startScale;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / fadeDuration);
            enemy.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        StartCoroutine(MoveEnemy(enemy));
    }

    private IEnumerator MoveEnemy(GameObject enemy)
    {
        while (enemy != null && Vector3.Distance(enemy.transform.position, endPoint.position) > 0.1f)
        {
            enemy.transform.position = Vector3.MoveTowards(
                enemy.transform.position,
                endPoint.position,
                walkSpeed * Time.deltaTime
            );
            yield return null;
        }

        // Once it hits the end, recycle
        if (enemy != null)
        {
            activeEnemies.Remove(enemy);
            Destroy(enemy);
        }
    }
}
