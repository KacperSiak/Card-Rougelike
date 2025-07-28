using UnityEngine;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    [Header("References")]
    public MapGenerator mapGenerator;

    [Header("Map Time Settings")]
    public float maxMapTime = 0f;
    public float currentMapTime = 0f;

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
        currentMapTime = maxMapTime;
        Debug.Log($"[MapManager] Max map time: {maxMapTime:F2}");
    }

    public void TravelToNode(Vector2 fromPos, Vector2 toPos)
    {
        float travelTime = Vector2.Distance(fromPos, toPos);
        currentMapTime -= travelTime;
        
        if (currentMapTime < 0f)
            currentMapTime = 0f;
            
        Debug.Log($"[MapManager] Traveled distance: {travelTime:F2}, remaining time: {currentMapTime:F2}");
    }

    public bool HasTimeLeft()
    {
        return currentMapTime > 0f;
    }
}
