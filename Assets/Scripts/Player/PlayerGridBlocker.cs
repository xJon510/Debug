using UnityEngine;
using System.Collections.Generic;

public class PlayerGridBlocker : MonoBehaviour
{
    public float cellSize = 2.5f;   // match GridManager
    public Vector2Int size = new Vector2Int(1, 1); // footprint of player in grid cells

    void Update()
    {
        Vector3 pos = transform.position;
        int baseX = Mathf.FloorToInt(pos.x / cellSize);
        int baseZ = Mathf.FloorToInt(pos.z / cellSize);

        List<Vector2Int> blocked = new List<Vector2Int>();

        for (int dx = 0; dx < size.x; dx++)
        {
            for (int dz = 0; dz < size.y; dz++)
            {
                blocked.Add(new Vector2Int(baseX + dx, baseZ + dz));
            }
        }

        GridManager.Instance.UpdatePlayerBlockedCells(blocked);
    }
}
