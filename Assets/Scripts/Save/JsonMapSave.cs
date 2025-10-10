using System;
using System.Collections.Generic;
using UnityEngine;
using MapSystem;

[Serializable]
public class JsonMapSave
{
    public string version = "1.0";
    public string savedAt;

    // Optional global info
    public int seed;
    public float maxMapTime;
    public float currentMapTime;

    public List<JsonNodeRecord> nodes = new List<JsonNodeRecord>();
    public List<JsonEdgeRecord> edges = new List<JsonEdgeRecord>();
}

[Serializable]
public class JsonNodeRecord
{
    public string guid;
    public NodeType type;
    public Vector2 position; // x,z
    public int levelIndex;
    public bool cleared;
}

[Serializable]
public class JsonEdgeRecord
{
    public string fromGuid;
    public string toGuid;
}
