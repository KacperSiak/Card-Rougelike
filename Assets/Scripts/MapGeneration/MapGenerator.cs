using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using MapSystem;

[System.Serializable]
public class NodeTypeSettings
{
    public NodeType type;
    public int minCount;
    public int maxCount;
    public float spawnChance;
    public GameObject prefab;
}

public class MapGenerator : MonoBehaviour
{
    [ExecuteInEditMode]
    [Header("Node Type Configuration")]
    public NodeTypeSettings[] nodeTypes;
    
    [Header("Boss Configuration")]
    public GameObject bossPrefab;

    [Header("Map Settings")]
    public int numberOfLevels = 8;
    public int minNodesPerLevel = 2;
    public int maxNodesPerLevel = 4;
    public float horizontalSpacing = 5f;
    public float verticalSpacing = 6f;
    
    [Header("Position Randomization")]
    public bool randomizePositions = true;
    public float horizontalRandomRange = 2f;
    public float verticalRandomRange = 1f;

    [Header("Connection Visualization")]
    public bool showConnections = true;
    public Material connectionMaterial;
    public float connectionWidth = 0.1f;
    
    private Material cachedLineMaterial;

    public List<List<MapNode>> mapLevels = new List<List<MapNode>>();
    
    private Dictionary<NodeType, int> currentCounts = new Dictionary<NodeType, int>();
    private Dictionary<NodeType, NodeTypeSettings> settingsLookup = new Dictionary<NodeType, NodeTypeSettings>();
    private bool bossAssigned = false;


    void Start()
    {
        InitializeSettings();
        GenerateMap();
    }

    void InitializeSettings()
    {
        if (nodeTypes == null || nodeTypes.Length == 0)
        {
            nodeTypes = new NodeTypeSettings[]
            {
                new NodeTypeSettings { type = NodeType.Enemy, minCount = 8, maxCount = 20, spawnChance = 0.6f },
                new NodeTypeSettings { type = NodeType.Elite, minCount = 2, maxCount = 5, spawnChance = 0.15f },
                new NodeTypeSettings { type = NodeType.Shop, minCount = 1, maxCount = 3, spawnChance = 0.05f },
                new NodeTypeSettings { type = NodeType.Treasure, minCount = 3, maxCount = 6, spawnChance = 0.15f }
            };
        }
        
        settingsLookup.Clear();
        currentCounts.Clear();
        
        foreach (var setting in nodeTypes)
        {
            settingsLookup[setting.type] = setting;
            currentCounts[setting.type] = 0;
        }
    }

    public void GenerateMap()
    {
        ClearOldNodes();
        ResetMapState();
        GenerateMapStructure();
        ConnectNodes();
        InstantiateMapVisuals();
        if (showConnections)
        {
            DrawConnections();
        }
        
        NodeClickHandler.ResetCache();
        NodeClickHandler.LoadPlayerPosition();
    }

    void ResetMapState()
    {
        bossAssigned = false;
        mapLevels.Clear();
        
        foreach (var type in currentCounts.Keys.ToList())
        {
            currentCounts[type] = 0;
        }
    }

    void GenerateMapStructure()
    {
        for (int level = 0; level < numberOfLevels; level++)
        {
            int nodesCount = Random.Range(minNodesPerLevel, maxNodesPerLevel + 1);
            List<MapNode> currentLevelNodes = new List<MapNode>();

            for (int i = 0; i < nodesCount; i++)
            {
                var availableTypes = GetAvailableNodeTypes();
                if (availableTypes.Count == 0 && level != numberOfLevels - 1)
                {
                    break;
                }

                float baseX = i * horizontalSpacing;
                float baseY = -level * verticalSpacing;

                if (randomizePositions)
                {
                    baseX += Random.Range(-horizontalRandomRange, horizontalRandomRange);
                    baseY += Random.Range(-verticalRandomRange, verticalRandomRange);
                }
                
                float totalWidth = (nodesCount - 1) * horizontalSpacing;
                float offsetX = -totalWidth / 2f;

                MapNode node = new MapNode
                {
                    Type = GetRandomNodeType(level),
                    Position = new Vector2(baseX + offsetX, baseY)
                };
                currentLevelNodes.Add(node);
            }
            mapLevels.Add(currentLevelNodes);
        }
    }

    void InstantiateMapVisuals()
    {
        foreach (var level in mapLevels)
        {
            foreach (var node in level)
            {
                GameObject prefab = GetPrefabForNodeType(node.Type);
                if (prefab != null)
                {
                    Vector3 pos3D = new Vector3(node.Position.x, 0, node.Position.y);
                    Instantiate(prefab, pos3D, Quaternion.identity, transform);
                }
            }
        }
    }

    NodeType GetRandomNodeType(int level)
    {
        if (level == numberOfLevels - 1 && !bossAssigned)
        {
            bossAssigned = true;
            return NodeType.Boss;
        }

        var availableTypes = GetAvailableNodeTypes();
        if (availableTypes.Count == 0)
        {
            return NodeType.Enemy;
        }

        float totalWeight = availableTypes.Sum(t => settingsLookup[t].spawnChance);
        float randomValue = Random.value * totalWeight;
        float currentWeight = 0f;

        foreach (var type in availableTypes)
        {
            currentWeight += settingsLookup[type].spawnChance;
            if (randomValue <= currentWeight)
            {
                currentCounts[type]++;
                return type;
            }
        }

        var fallbackType = availableTypes.First();
        currentCounts[fallbackType]++;
        return fallbackType;
    }

    List<NodeType> GetAvailableNodeTypes()
    {
        return settingsLookup.Keys
            .Where(type => currentCounts[type] < settingsLookup[type].maxCount)
            .ToList();
    }

    void ConnectNodes()
    {
        for (int level = 0; level < mapLevels.Count - 1; level++)
        {
            List<MapNode> currentLevel = mapLevels[level];
            List<MapNode> nextLevel = mapLevels[level + 1];

            foreach (var node in currentLevel)
            {
                int connections = Random.Range(1, Mathf.Min(3, nextLevel.Count + 1));
                List<int> connectedIndices = new List<int>();

                for (int i = 0; i < connections; i++)
                {
                    int index;
                    do
                    {
                        index = Random.Range(0, nextLevel.Count);
                    } while (connectedIndices.Contains(index));

                    connectedIndices.Add(index);
                    MapNode targetNode = nextLevel[index];
                    
                    if (!node.ConnectedNodes.Contains(targetNode))
                        node.ConnectedNodes.Add(targetNode);
                    if (!targetNode.ConnectedNodes.Contains(node))
                        targetNode.ConnectedNodes.Add(node);
                }
            }
        }
        
        for (int level = 0; level < mapLevels.Count; level++)
        {
            List<MapNode> currentLevel = mapLevels[level];
            
            for (int i = 0; i < currentLevel.Count - 1; i++)
            {
                MapNode nodeA = currentLevel[i];
                MapNode nodeB = currentLevel[i + 1];
                
                if (Random.value < 0.5f)
                {
                    if (!nodeA.ConnectedNodes.Contains(nodeB))
                        nodeA.ConnectedNodes.Add(nodeB);
                    if (!nodeB.ConnectedNodes.Contains(nodeA))
                        nodeB.ConnectedNodes.Add(nodeA);
                }
            }
        }
    }

    void DrawConnections()
    {
        HashSet<string> drawnConnections = new HashSet<string>();
        
        foreach (var level in mapLevels)
        {
            foreach (var node in level)
            {
                Vector3 startPos = new Vector3(node.Position.x, 0.1f, node.Position.y);

                foreach (var connectedNode in node.ConnectedNodes)
                {
                    Vector3 endPos = new Vector3(connectedNode.Position.x, 0.1f, connectedNode.Position.y);
                    
                    string connectionKey = $"{Mathf.Min(startPos.x, endPos.x)}_{Mathf.Min(startPos.z, endPos.z)}_{Mathf.Max(startPos.x, endPos.x)}_{Mathf.Max(startPos.z, endPos.z)}";
                    
                    if (!drawnConnections.Contains(connectionKey))
                    {
                        CreateConnectionLine(startPos, endPos, $"Connection_{connectionKey}");
                        drawnConnections.Add(connectionKey);
                    }
                }
            }
        }
    }

    void CreateConnectionLine(Vector3 startPos, Vector3 endPos, string name)
    {
        GameObject lineObj = new GameObject(name);
        lineObj.transform.SetParent(transform);
        
        LineRenderer line = lineObj.AddComponent<LineRenderer>();
        line.material = GetLineMaterial();
        line.startWidth = connectionWidth;
        line.endWidth = connectionWidth;
        line.positionCount = 2;
        line.useWorldSpace = true;
        line.sortingOrder = -1;
        
        line.SetPosition(0, startPos);
        line.SetPosition(1, endPos);
    }

    Material GetLineMaterial()
    {
        if (connectionMaterial != null)
            return connectionMaterial;
            
        if (cachedLineMaterial == null)
        {
            cachedLineMaterial = CreateDefaultLineMaterial();
        }
        
        return cachedLineMaterial;
    }

    Material CreateDefaultLineMaterial()
    {
        Material mat = new Material(Shader.Find("Unlit/Color"));
        if (mat.shader == null)
        {
            mat = new Material(Shader.Find("Standard"));
        }
        mat.color = new Color(0.8f, 0.8f, 0.8f, 0.7f);
        return mat;
    }

    GameObject GetPrefabForNodeType(NodeType type)
    {
        if (type == NodeType.Boss)
            return bossPrefab;
            
        return settingsLookup.ContainsKey(type) ? settingsLookup[type].prefab : null;
    }
    
    void ClearOldNodes()
    {
        var childrenToDestroy = new List<GameObject>();
        
        foreach (Transform child in transform)
        {
            childrenToDestroy.Add(child.gameObject);
        }
        
        foreach (var child in childrenToDestroy)
        {
            if (child != null)
            {
                if (Application.isPlaying)
                    Destroy(child);
                else
                    DestroyImmediate(child);
            }
        }
    }

    [ContextMenu("Regenerate Map")]
    public void RegenerateMap()
    {
        GenerateMap();
    }

    [ContextMenu("Validate Settings")]
    public void ValidateSettings()
    {
        foreach (var setting in nodeTypes)
        {
            if (setting.minCount > setting.maxCount)
            {
                Debug.LogWarning($"{setting.type}: minCount ({setting.minCount}) > maxCount ({setting.maxCount})");
            }
            if (setting.prefab == null)
            {
                Debug.LogWarning($"{setting.type}: Missing prefab reference");
            }
        }
        
        float totalChance = nodeTypes.Sum(t => t.spawnChance);
        if (Mathf.Abs(totalChance - 1f) > 0.01f)
        {
            Debug.LogWarning($"Total spawn chances = {totalChance:F2}, should be close to 1.0");
        }
    }
}