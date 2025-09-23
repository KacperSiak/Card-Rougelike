using System; // Kacper Added
using UnityEngine;
using System.Collections.Generic;
using System.Data;

public class MapManager : PersistenSingleton<MapManager>
{
    [Header("References")]
    public MapGenerator mapGenerator;

    [Header("Map Time Settings")]
    public float maxMapTime = 0f;
    public float currentMapTime = 0f;

    // Kacper Added
    private float _modifier = 1;

    protected override void OnSingletonAwake() { }
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
    void Start()
    {
        if (mapGenerator == null)
            mapGenerator = FindFirstObjectByType<MapGenerator>(); //tymczasowe
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

    public void TravelToNode(Vector2 fromPos, Vector2 toPos) // TODO: zmienic na guid
    {
        float travelTime = (float)Math.Round(Vector2.Distance(fromPos, toPos) * _modifier);
        currentMapTime -= travelTime;
        
        if (currentMapTime < 0f)
            currentMapTime = 0f;
            
        Debug.Log($"[MapManager] Traveled distance: {travelTime:F2}, remaining time: {currentMapTime:F2}");
    }

    public bool HasTimeLeft()
    {
        return currentMapTime > 0f;
    }
    
    public void TeleportToBossWhenTimeUp()
    {
        Debug.Log($"[MapManager] TeleportToBossWhenTimeUp called. Current time: {currentMapTime}");
        
        if (currentMapTime > 0f) 
        {
            Debug.Log("[MapManager] Still has time left, not teleporting");
            return;
        }
        
        if (mapGenerator == null)
        {
            Debug.LogWarning("[MapManager] No MapGenerator reference for boss teleport");
            return;
        }
        
        Transform bossNode = mapGenerator.GetBossNodeTransform();
        if (bossNode == null)
        {
            Debug.LogWarning("[MapManager] Boss node not found for teleport");
            return;
        }
        
        Debug.Log($"[MapManager] Boss node found at position: {bossNode.position}");
        
        if (NodeClickHandler.playerMarker != null)
        {
            Vector3 fromPos = Vector3.zero;
            bool hasFromPosition = false;
            
            if (NodeClickHandler.currentPlayerNode != null)
            {
                fromPos = NodeClickHandler.currentPlayerNode.position;
                hasFromPosition = true;
                Debug.Log($"[MapManager] Moving from position: {fromPos}");
            }
            
            Vector3 bossPos = bossNode.position;
            bossPos.y = 0.2f;
            NodeClickHandler.playerMarker.transform.position = bossPos;
            NodeClickHandler.currentPlayerNode = bossNode;
            
            Debug.Log($"[MapManager] Player marker teleported to: {bossPos}");
            
            if (hasFromPosition)
            {
                Vector3 direction = (bossPos - fromPos).normalized;
                if (direction != Vector3.zero)
                {
                    float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                    NodeClickHandler.playerMarker.transform.rotation = Quaternion.Euler(0, angle, 0);
                }
            }
            
            Debug.Log("[MapManager] Player teleported to Boss due to time running out!");
            
            var nodeHandler = bossNode.GetComponent<NodeClickHandler>();
            if (nodeHandler != null)
            {
                Debug.Log("[MapManager] Auto-loading Boss scene...");
                nodeHandler.TriggerLoadScene();
            }
            else
            {
                Debug.LogWarning("[MapManager] No NodeClickHandler found on boss node!");
            }
        }
        else
        {
            Debug.LogWarning("[MapManager] No player marker found for teleport!");
        }
    }

    // Kacper Added
    public void SetNewModifier(float newModifier)
    {
        _modifier = newModifier;
    }

    public void ResetModifier()
    {
        _modifier = 1;
    }

    public void ModifyTime(float time, bool add)
    {
        if (add) currentMapTime += time;
        else currentMapTime -= time;
    }

    public void AutoTeleport()
    {
        currentMapTime = 0;
    }
}
