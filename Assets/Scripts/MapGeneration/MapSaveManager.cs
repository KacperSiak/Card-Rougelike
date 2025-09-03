using UnityEngine;
using System.Collections;

public class MapSaveManager : MonoBehaviour
{
    [Header("Save Data")]
    public MapSaveDataSO mapSaveData;
    
    [Header("References")]
    public MapGenerator mapGenerator;
    public MapManager mapManager;
    
    void Start()
    {
        if (mapGenerator == null)
            mapGenerator = FindFirstObjectByType<MapGenerator>();
        if (mapManager == null)
            mapManager = FindFirstObjectByType<MapManager>(); //tymczasowe
        
        if (GameSaveManager.Instance != null && GameSaveManager.Instance.HasActiveSave())
        {
            mapSaveData = GameSaveManager.Instance.GetActiveSave();
            Debug.Log("[MapSaveManager] Using active save from GameSaveManager");
        }
            
        if (mapSaveData != null && mapSaveData.CanLoadMap())
        {
            Debug.Log("[MapSaveManager] Loading saved map...");
            LoadSavedMap();
        }
        else
        {
            if (mapSaveData == null)
            {
                CreateDefaultSave();
            }
            Debug.Log("[MapSaveManager] No save found or creating new map");
        }
    }
    
    public void SaveCurrentMap()
    {
        if (mapSaveData == null)
        {
            Debug.LogWarning("[MapSaveManager] No MapSaveDataSO assigned");
            return;
        }
        
        if (mapGenerator == null || mapManager == null)
        {
            Debug.LogWarning("[MapSaveManager] Missing reference to MapGenerator or MapManager");
            return;
        }
        
        mapSaveData.SaveCurrentMapState(mapGenerator, mapManager);
        Debug.Log("[MapSaveManager] Map saved to ScriptableObject");
    }
    
    public void LoadSavedMap()
    {
        if (mapSaveData == null || !mapSaveData.CanLoadMap())
        {
            Debug.LogWarning("[MapSaveManager] No valid save to load");
            return;
        }
        
        if (mapGenerator == null)
        {
            Debug.LogWarning("[MapSaveManager] No MapGenerator reference");
            return;
        }
        
        mapSaveData.ApplyToMapGenerator(mapGenerator);
        
        mapGenerator.GenerateMap();
        
        if (mapManager != null)
        {
            mapSaveData.ApplyToMapManager(mapManager);
        }
        
        if (mapSaveData.hasPlayerPosition)
        {
            StartCoroutine(RestorePlayerPositionDelayed());
        }
        
        Debug.Log($"[MapSaveManager] Map loaded from: {mapSaveData.saveDate}");
    }
    
    IEnumerator RestorePlayerPositionDelayed()
    {
        yield return new WaitForSeconds(0.2f);
        
        Transform closestNode = null;
        float closestDistance = float.MaxValue;
        
        var allNodes = FindObjectsByType<NodeClickHandler>(FindObjectsSortMode.None);
        
        foreach (var nodeHandler in allNodes)
        {
            Vector2 nodePos = new Vector2(nodeHandler.transform.position.x, nodeHandler.transform.position.z);
            float distance = Vector2.Distance(nodePos, mapSaveData.playerPosition);
            
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNode = nodeHandler.transform;
            }
        }
        
        if (closestNode != null)
        {
            NodeClickHandler.currentPlayerNode = closestNode;
            
            if (NodeClickHandler.playerMarker != null)
            {
                Vector3 newPos = closestNode.position;
                newPos.y = 0.2f;
                NodeClickHandler.playerMarker.transform.position = newPos;
                NodeClickHandler.playerMarker.SetActive(true);
            }
            
            Debug.Log($"[MapSaveManager] Player restored to position: {closestNode.position}");
        }
    }
    
    public bool HasSavedMap()
    {
        return mapSaveData != null && mapSaveData.CanLoadMap();
    }
    
    public void DeleteSavedMap()
    {
        if (mapSaveData != null)
        {
            mapSaveData.ClearSave();
            Debug.Log("[MapSaveManager] Save deleted");
        }
    }
    
    private void CreateDefaultSave()
    {
        mapSaveData = ScriptableObject.CreateInstance<MapSaveDataSO>();
        
        mapSaveData.numberOfLevels = 8;
        mapSaveData.minNodesPerLevel = 2;
        mapSaveData.maxNodesPerLevel = 4;
        mapSaveData.horizontalSpacing = 5f;
        mapSaveData.verticalSpacing = 6f;
        mapSaveData.horizontalRandomRange = 2f;
        mapSaveData.verticalRandomRange = 1f;
        mapSaveData.mapSeed = 0;
        mapSaveData.hasPlayerPosition = false;
        mapSaveData.maxMapTime = 0f;
        mapSaveData.currentMapTime = 0f;
        mapSaveData.saveDate = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        
        Debug.Log("[MapSaveManager] Default save created");
    }
}
