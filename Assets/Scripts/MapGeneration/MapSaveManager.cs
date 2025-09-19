using UnityEngine;
using System.Collections;

public class MapSaveManager : MonoBehaviour
{
    [Header("Save Data")]
    public MapSaveDataSO mapSaveData;

	[Header("References")]
    public MapGenerator mapGenerator;
    //consider using this pattern
    //[SerializeField] private MapGenerator mapGenerator;
    //this prevents any changes form outside while allowing the field to be drawn to inspector
    public MapManager mapManager;
    
    void Start()
    {
        if (mapGenerator == null)
            Debug.LogWarning("[MapSaveManager] MapGenerator not assigned (Inspector)");
        if (mapManager == null)
            Debug.LogWarning("[MapSaveManager] MapManager not assigned (Inspector)");
        
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

    void OnEnable()
    {
        if (mapSaveData != null && mapSaveData.hasPlayerPosition)
        {
            if (NodeClickHandler.playerMarker != null)
                NodeClickHandler.playerMarker.SetActive(false);

            if (!TryRestorePlayerPositionImmediate())
            {
                StartCoroutine(RestoreWhenReady());
            }
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

    public void SavePlayerPositionOnly()
    {
        if (mapSaveData == null || mapManager == null) return;
        if (NodeClickHandler.currentPlayerNode != null)
        {
            Vector3 pos = NodeClickHandler.currentPlayerNode.position;
            mapSaveData.playerPosition = new Vector2(pos.x, pos.z);
            mapSaveData.hasPlayerPosition = true;
            mapSaveData.currentMapTime = mapManager.currentMapTime;
            mapSaveData.maxMapTime = mapManager.maxMapTime;
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(mapSaveData);
            #endif
        }
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

        if (NodeClickHandler.playerMarker != null)
            NodeClickHandler.playerMarker.SetActive(false);

        mapGenerator.GenerateMap();
        
        if (mapManager != null)
        {
            mapSaveData.ApplyToMapManager(mapManager);
        }
        
        if (mapSaveData.hasPlayerPosition)
        {
            TryRestorePlayerPositionImmediate();
        }
        
        Debug.Log($"[MapSaveManager] Map loaded from: {mapSaveData.saveDate}");
    }
    
    bool TryRestorePlayerPositionImmediate()
    {
        if (mapSaveData == null || mapGenerator == null) return false;

        Transform closestNode = mapGenerator.GetClosestNodeTransform2D(mapSaveData.playerPosition);
        if (closestNode == null) return false;

        NodeClickHandler.currentPlayerNode = closestNode;

        if (NodeClickHandler.playerMarker != null)
        {
            Vector3 newPos = closestNode.position;
            newPos.y = 0.2f;
            NodeClickHandler.playerMarker.transform.position = newPos;
            NodeClickHandler.playerMarker.SetActive(true);
        }

        Debug.Log($"[MapSaveManager] Player restored to position: {closestNode.position}");
        return true;
    }

    IEnumerator RestoreWhenReady()
    {
        while (mapGenerator != null && mapSaveData != null)
        {
            if (TryRestorePlayerPositionImmediate())
                yield break;
            yield return null;
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
