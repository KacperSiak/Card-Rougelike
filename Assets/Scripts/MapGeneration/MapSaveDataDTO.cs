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

    public Vector2 playerPosition;
    public bool hasPlayerPosition;

    public float maxMapTime;
    public float currentMapTime;

    public List<string> clearedCombatGuids = new List<string>();

    public string saveDate;

    public bool CanLoadMap()
    {
        return mapSeed != 0;
    }
}