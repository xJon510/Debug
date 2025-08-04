using UnityEngine;

public enum SpawnMode { Immediate, Staggered, Burst }
// Immediate - Spawns all enemies at once

// Staggered - Spawns enemies one at a time with a delay

// Burst - Spawns a group of enemies at once, then waits before the next group


public enum SpawnDistribution { Even, RoundRobin, Random, SingleSide, Weighted }

// Even - Divides enemies evenly across spawn points
//        Example - 10 Worms -> 2, 2, 3, 3

// RoundRobin - Spawns one enemy at each point in sequence
//        Example - 10 Worms -> 1 from P1, 1 from P2, 1 from P3, 1 from P4, repeat until all are spawned

// Random - Spawns enemies at random points
//        Example - 10 Worms -> 5 might end up at P1, 1 at P2, 3 at P3, 1 at P4

// SingleSide - Spawns all enemies at one side of the map
//        Good for "Floodgates Open" pressure or Boss/finale waves

// Weighted - Uses spawnWeights to determine how many enemies spawn at each point
//        Example - P1 = 50%, p2 = 25%, P3 = 15%, P4 = 10%


[System.Serializable]
public class WaveConfig
{
    public GameObject enemyPrefab;   // which enemy to spawn
    public int enemyCount = 10;      // total enemies
    public SpawnMode spawnMode = SpawnMode.Immediate;
    public float spawnRate = 1f;     // time between spawns (stagger/burst)
    public int burstSize = 3;        // group size for Burst mode
    public SpawnDistribution distribution = SpawnDistribution.RoundRobin;
    public float[] spawnWeights;     // used if Weighted distribution
}
