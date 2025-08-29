using UnityEngine;
using System.Collections.Generic;
using MapSystem;

[System.Serializable]
public class SerializableMapNode
{
    public NodeType nodeType;
    public Vector2 position;
    public List<int> connectedNodeIndices = new List<int>();
    public string sceneToLoad = "";
    
    public SerializableMapNode() { }
    
    public SerializableMapNode(MapNode node, int nodeIndex, Dictionary<MapNode, int> nodeToIndex)
    {
        nodeType = node.Type;
        position = node.Position;
        
        foreach (var connectedNode in node.ConnectedNodes)
        {
            if (nodeToIndex.ContainsKey(connectedNode))
            {
                connectedNodeIndices.Add(nodeToIndex[connectedNode]);
            }
        }
    }
}

[System.Serializable]
public class MapSaveData
{
    [Header("Generation Data")]
    public int randomSeed;
    public int lastUsedSeed;
    public int numberOfLevels;
    public Random.State randomState;
    
    [Header("Map Structure")]
    public List<List<SerializableMapNode>> mapLevels = new List<List<SerializableMapNode>>();
    
    [Header("Player Data")]
    public Vector2 currentPlayerPosition;
    public bool hasPlayerPosition = false;
    
    [Header("Time Management")]
    public float maxMapTime;
    public float currentMapTime;
    
    [Header("Save Info")]
    public string saveDate;
    public int saveVersion = 1;
    
    public MapSaveData()
    {
        saveDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
