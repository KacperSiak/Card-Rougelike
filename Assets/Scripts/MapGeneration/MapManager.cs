using UnityEngine;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    [Header("References")]
    public MapGenerator mapGenerator;

    [Header("Map Time Settings")]
    public float maxMapTime = 0f;

    void Start()
    {
        if (mapGenerator == null)
            mapGenerator = FindObjectOfType<MapGenerator>(); // tymczasowe
    }

    public void CalculateMaxMapTime()
    {
        if (mapGenerator == null || mapGenerator.mapLevels == null)
            return;

        float totalDistance = 0f;
        var visited = new HashSet<string>();

        foreach (var level in mapGenerator.mapLevels)
        {
            foreach (var node in level)
            {
                foreach (var connected in node.ConnectedNodes)
                {
                    string key = node.Position.ToString() + "_" + connected.Position.ToString();
                    string keyRev = connected.Position.ToString() + "_" + node.Position.ToString();
                    if (!visited.Contains(key) && !visited.Contains(keyRev))
                    {
                        totalDistance += Vector2.Distance(node.Position, connected.Position);
                        visited.Add(key);
                    }
                }
            }
        }
        maxMapTime = totalDistance;
        Debug.Log($"[MapManager] Max map time: {maxMapTime:F2}");
    }
}
