using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapSaveDataDTO
{
    public int mapSeed;
    public int numberOfLevels;
    public int minNodesPerLevel;
    public int maxNodesPerLevel;
    public float horizontalSpacing;
    public float verticalSpacing;
    public float horizontalRandomRange;
    public float verticalRandomRange;

    // Player
    public Vector2 playerPosition;
    public bool hasPlayerPosition;

    // Time management
    public float maxMapTime;
    public float currentMapTime;

    // Cleared combat nodes (GUIDs)
    public List<string> clearedCombatGuids = new List<string>();

    // Meta
    public string saveDate;

    public bool CanLoadMap()
    {
        return mapSeed != 0;
    }
}