using System;
using System.Collections.Generic;
using UnityEngine;
using MapSystem;

[Serializable]
public class JsonMapSave
{
    public string version = "1.0";
    public string savedAt;
    public string slotName;

    public int seed;
    public float maxMapTime;
    public float currentMapTime;
    
    public float playTimeSeconds;
    public int totalNodesCleared;
    public int totalCombatsWon;

    public List<JsonNodeRecord> nodes = new List<JsonNodeRecord>();
    public List<JsonEdgeRecord> edges = new List<JsonEdgeRecord>();
}

[Serializable]
public class JsonNodeRecord
{
    public string guid;
    public NodeType type;
    public Vector2 position;
    public int levelIndex;
    public bool cleared;
}

[Serializable]
public class JsonEdgeRecord
{
    public string fromGuid;
    public string toGuid;
}
