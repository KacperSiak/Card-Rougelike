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
    
    [Header("Settings")]
    public bool autoSaveBeforeSceneChange = true;
    public bool autoLoadOnStart = true;
    
    void Start()
    {
        if (mapGenerator == null)
            mapGenerator = FindFirstObjectByType<MapGenerator>();
        if (mapManager == null)
            mapManager = FindFirstObjectByType<MapManager>(); //tymczasowe [mam nadzieje]
            
        if (autoLoadOnStart && mapSaveData != null && mapSaveData.CanLoadMap())
        {
            Debug.Log("[MapSaveManager] Loading saved map...");
            LoadSavedMap();
        }
        else
        {
            Debug.Log("[MapSaveManager] No save found - a new map will be generated");
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
    
    public void SaveBeforeSceneChange()
    {
        if (autoSaveBeforeSceneChange)
        {
            SaveCurrentMap();
        }
    }
    
    [ContextMenu("Save Current Map")]
    public void SaveFromMenu()
    {
        SaveCurrentMap();
    }
    
    [ContextMenu("Load Saved Map")]
    public void LoadFromMenu()
    {
        LoadSavedMap();
    }
    
    [ContextMenu("Delete Saved Map")]
    public void DeleteFromMenu()
    {
        DeleteSavedMap();
    }
    
    [ContextMenu("Create New Save Data Asset")]
    public void CreateSaveDataAsset()
    {
        #if UNITY_EDITOR
        var saveData = ScriptableObject.CreateInstance<MapSaveDataSO>();
        UnityEditor.AssetDatabase.CreateAsset(saveData, "Assets/MapSaveData.asset");
        UnityEditor.AssetDatabase.SaveAssets();
        mapSaveData = saveData;
        Debug.Log("Created new MapSaveDataSO asset");
        #endif
    }
}
