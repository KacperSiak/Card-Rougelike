using UnityEngine;
using System.Collections.Generic;
using MapSystem;

[CreateAssetMenu(fileName = "MapSaveData", menuName = "Map System/Map Save Data")]
public class MapSaveDataSO : ScriptableObject
{
    [Header("Generation Data")]
    public int mapSeed;
    public int numberOfLevels;
    
    [Header("Player Data")]
    public Vector2 playerPosition;
    public bool hasPlayerPosition = false;
    
    [Header("Time Management")]
    public float maxMapTime;
    public float currentMapTime;
    
    [Header("Map Settings")]
    public int minNodesPerLevel = 2;
    public int maxNodesPerLevel = 4;
    public float horizontalSpacing = 5f;
    public float verticalSpacing = 6f;
    public float horizontalRandomRange = 2f;
    public float verticalRandomRange = 1f;
    
    [Header("Save Info")]
    public string saveDate;
    public bool isValid = false;
    
    public void SaveCurrentMapState(MapGenerator mapGen, MapManager mapMgr)
    {
        if (mapGen == null) return;
        
        mapSeed = mapGen.lastUsedSeed;
        numberOfLevels = mapGen.numberOfLevels;
        
        minNodesPerLevel = mapGen.minNodesPerLevel;
        maxNodesPerLevel = mapGen.maxNodesPerLevel;
        horizontalSpacing = mapGen.horizontalSpacing;
        verticalSpacing = mapGen.verticalSpacing;
        horizontalRandomRange = mapGen.horizontalRandomRange;
        verticalRandomRange = mapGen.verticalRandomRange;
        
        if (NodeClickHandler.currentPlayerNode != null)
        {
            Vector3 playerPos = NodeClickHandler.currentPlayerNode.position;
            playerPosition = new Vector2(playerPos.x, playerPos.z);
            hasPlayerPosition = true;
        }
        else
        {
            hasPlayerPosition = false;
        }
        
        if (mapMgr != null)
        {
            maxMapTime = mapMgr.maxMapTime;
            currentMapTime = mapMgr.currentMapTime;
        }
        
        isValid = true;
        saveDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        
        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
        
        Debug.Log($"[MapSaveDataSO] Zapisano mapę z seed: {mapSeed}");
    }
    
    public bool CanLoadMap()
    {
        return isValid && mapSeed != 0;
    }
    
    public void ApplyToMapGenerator(MapGenerator mapGen)
    {
        if (mapGen == null || !CanLoadMap()) return;
        
        mapGen.fixedSeed = mapSeed;
        mapGen.numberOfLevels = numberOfLevels;
        mapGen.minNodesPerLevel = minNodesPerLevel;
        mapGen.maxNodesPerLevel = maxNodesPerLevel;
        mapGen.horizontalSpacing = horizontalSpacing;
        mapGen.verticalSpacing = verticalSpacing;
        mapGen.horizontalRandomRange = horizontalRandomRange;
        mapGen.verticalRandomRange = verticalRandomRange;
    }
    
    public void ApplyToMapManager(MapManager mapMgr)
    {
        if (mapMgr == null || !CanLoadMap()) return;
        
        mapMgr.maxMapTime = maxMapTime;
        mapMgr.currentMapTime = currentMapTime;
    }
    
    public void ClearSave()
    {
        isValid = false;
        mapSeed = 0;
        hasPlayerPosition = false;
        saveDate = "";
        
        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
        
        Debug.Log("[MapSaveDataSO] Zapis wyczyszczony");
    }
}
