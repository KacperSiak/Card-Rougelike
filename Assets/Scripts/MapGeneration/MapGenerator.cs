using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using MapSystem;

[System.Serializable]
public struct NodeTypeSettings
{
    public NodeType type;
    public int minCount;
    public int maxCount;
    public GameObject prefab;
    public int minLevel;
}

public class MapGenerator : MonoBehaviour
{
    [Header("Node Type Configuration")]
    public NodeTypeSettings[] nodeTypes;

    [Header("Start Node Configuration")]
    public GameObject startNodePrefab;

    [Header("Player Configuration")]
    public GameObject playerMarkerPrefab;
    
    [HideInInspector]
    public GameObject currentPlayerMarker;

    [Header("Boss Configuration")]
    public GameObject bossPrefab;

    [Header("Map Settings")]
    public int numberOfLevels = 8;
    //[field: SerializeField] public int NumberOfLevels { get; private set; } = 8;
    public int minNodesPerLevel = 2;
    public int maxNodesPerLevel = 4;
    public float horizontalSpacing = 5f;
    public float verticalSpacing = 6f;

    [Header("Node Position Randomization")]
    [Tooltip("Max random offset for node X position")]
    public float horizontalRandomRange = 2f;
    [Tooltip("Max random offset for node Y position")]
    public float verticalRandomRange = 1f;

    [Header("Connection Visualization")]
    public bool showConnections = true;
    public Material connectionMaterial;
    public float connectionWidth = 0.1f;

    [Header("Node Connections")]
    public int minConnectionsPerNode = 1;
    public int maxConnectionsPerNode = 2;
    public float maxConnectionDistance = 8f;

    [Header("Camera")]
    public CameraController cameraController;

    public List<List<MapNode>> mapLevels = new List<List<MapNode>>();

    private Dictionary<NodeType, int> currentCounts = new Dictionary<NodeType, int>();
    private Dictionary<NodeType, NodeTypeSettings> settingsLookup = new Dictionary<NodeType, NodeTypeSettings>();
    private bool bossAssigned = false;

    [Header("References")]
    public MapManager mapManager;

    void Start()
    {
        InitializeSettings();
        GenerateMap();
    }

    void InitializeSettings()
    {
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
        SpawnPlayerMarker();
        DrawConnections();
        SetupCameraBounds();
        if (mapManager != null)
            mapManager.CalculateMaxMapTime();
    }

    void SetupCameraBounds()
    {
        if (cameraController != null)
        {
            cameraController.SetBoundsFromMap(this);
        }
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
            List<MapNode> currentLevelNodes = new List<MapNode>();
            int nodesCount = Random.Range(minNodesPerLevel, maxNodesPerLevel + 1);

            if (level == 0)
            {
                MapNode startNode = new MapNode
                {
                    Type = NodeType.Start,
                    Position = new Vector2(0, 0)
                };
                currentLevelNodes.Add(startNode);
                mapLevels.Add(currentLevelNodes);
                continue;
            }

            for (int i = 0; i < nodesCount; i++)
            {
                var availableTypes = new List<NodeType>();
                foreach (var kvp in settingsLookup)
                {
                    NodeType type = kvp.Key;
                    NodeTypeSettings settings = kvp.Value;
                    if (level < settings.minLevel)
                        continue;
                    if (currentCounts[type] < settings.maxCount)
                    {
                        availableTypes.Add(type);
                    }
                }

                if (i > 0)
                {
                    NodeType prevType = currentLevelNodes[i - 1].Type;
                    availableTypes.Remove(prevType);
                }

                if (availableTypes.Count == 0)
                {
                    foreach (var kvp in settingsLookup)
                    {
                        NodeType type = kvp.Key;
                        NodeTypeSettings settings = kvp.Value;
                        if (level < settings.minLevel)
                            continue;
                        if (currentCounts[type] < settings.maxCount)
                        {
                            availableTypes.Add(type);
                        }
                    }
                }

                if (availableTypes.Count == 0 && level != numberOfLevels - 1)
                {
                    break;
                }

                float baseX = i * horizontalSpacing;
                float baseY = -level * verticalSpacing;

                // Use dedicated randomization ranges
                baseX += Random.Range(-horizontalRandomRange, horizontalRandomRange);
                baseY += Random.Range(-verticalRandomRange, verticalRandomRange);

                float totalWidth = (nodesCount - 1) * horizontalSpacing;
                float offsetX = -totalWidth / 2f;

                MapNode node = new MapNode
                {
                    Type = GetRandomNodeType(level, availableTypes),
                    Position = new Vector2(baseX + offsetX, baseY)
                };
                currentLevelNodes.Add(node);
            }
            mapLevels.Add(currentLevelNodes);
        }
    }

    void InstantiateMapVisuals()
    {
        for (int level = 0; level < mapLevels.Count; level++)
        {
            foreach (var node in mapLevels[level])
            {
                GameObject prefab = null;
                if (level == 0 && startNodePrefab != null && node.Type == NodeType.Start)
                {
                    prefab = startNodePrefab;
                }
                else
                {
                    prefab = GetPrefabForNodeType(node.Type);
                }
                if (prefab != null)
                {
                    Vector3 pos3D = new Vector3(node.Position.x, 0, node.Position.y);
                    Instantiate(prefab, pos3D, Quaternion.identity, transform);
                }
            }
        }
    }

    void SpawnPlayerMarker()
    {
        if (playerMarkerPrefab == null || mapLevels.Count == 0) return;

        if (mapLevels[0].Count > 0)
        {
            MapNode startNode = mapLevels[0][0];
            Vector3 playerPos = new Vector3(startNode.Position.x, 0.2f, startNode.Position.y);
            
            if (currentPlayerMarker == null)
            {
                currentPlayerMarker = Instantiate(playerMarkerPrefab, playerPos, Quaternion.identity, transform);
                currentPlayerMarker.name = "PlayerMarker";
            }
            else
            {
                currentPlayerMarker.transform.position = playerPos;
            }
            
            NodeClickHandler.playerMarker = currentPlayerMarker;
            NodeClickHandler.currentPlayerNode = GetStartNodeTransform();
        }
    }

    Transform GetStartNodeTransform()
    {
        foreach (Transform child in transform)
        {
            if (child.name.Contains("Start") || 
                (mapLevels.Count > 0 && Vector3.Distance(child.position, 
                    new Vector3(mapLevels[0][0].Position.x, 0, mapLevels[0][0].Position.y)) < 0.1f))
            {
                return child;
            }
        }
        return null;
    }

    NodeType GetRandomNodeType(int level, List<NodeType> availableTypes)
    {
        if (level == numberOfLevels - 1 && !bossAssigned)
        {
            bossAssigned = true;
            return NodeType.Boss;
        }

        if (availableTypes == null || availableTypes.Count == 0)
            return NodeType.Enemy;

        var selectedType = availableTypes[Random.Range(0, availableTypes.Count)];
        currentCounts[selectedType]++;
        return selectedType;
    }

    void ConnectNodes()
    {
        for (int level = 0; level < mapLevels.Count - 1; level++)
        {
            List<MapNode> currentLevel = mapLevels[level];
            List<MapNode> nextLevel = mapLevels[level + 1];

            foreach (var node in currentLevel)
            {
                var sortedNextNodes = nextLevel
                    .Select((n, index) => new { Node = n, Index = index, Distance = Vector2.Distance(node.Position, n.Position) })
                    .Where(x => x.Distance <= maxConnectionDistance)
                    .OrderBy(x => x.Distance)
                    .ToList();

                if (sortedNextNodes.Count == 0)
                {
                    sortedNextNodes = nextLevel
                        .Select((n, index) => new { Node = n, Index = index, Distance = Vector2.Distance(node.Position, n.Position) })
                        .OrderBy(x => x.Distance)
                        .Take(1)
                        .ToList();
                }

                bool isNextLevelBoss = (level + 1) == numberOfLevels - 1;
                int connectionsToMake = isNextLevelBoss ? 1 : Mathf.Min(2, sortedNextNodes.Count);

                for (int i = 0; i < connectionsToMake; i++)
                {
                    var targetInfo = sortedNextNodes[i];
                    MapNode targetNode = targetInfo.Node;

                    if (!node.ConnectedNodes.Contains(targetNode))
                        node.ConnectedNodes.Add(targetNode);
                    if (!targetNode.ConnectedNodes.Contains(node))
                        targetNode.ConnectedNodes.Add(node);
                }
            }
        }

        if (mapLevels.Count >= 2)
        {
            EnsureBossSingleConnection();
        }

        EnsureAllNodesConnected();
    }

    void EnsureBossSingleConnection()
    {
        int bossLevel = numberOfLevels - 1;
        if (bossLevel >= mapLevels.Count) return;

        List<MapNode> bossNodes = mapLevels[bossLevel];
        List<MapNode> preBossLevel = mapLevels[bossLevel - 1];

        foreach (var bossNode in bossNodes)
        {
            if (bossNode.Type == NodeType.Boss)
            {
                foreach (var preBossNode in preBossLevel)
                {
                    preBossNode.ConnectedNodes.RemoveAll(n => n == bossNode);
                }
                bossNode.ConnectedNodes.Clear();

                var closestPreBossNode = preBossLevel
                    .OrderBy(n => Vector2.Distance(n.Position, bossNode.Position))
                    .First();

                closestPreBossNode.ConnectedNodes.Add(bossNode);
                bossNode.ConnectedNodes.Add(closestPreBossNode);
            }
        }
    }

    void EnsureAllNodesConnected()
    {
        for (int level = 1; level < mapLevels.Count; level++)
        {
            List<MapNode> currentLevel = mapLevels[level];
            List<MapNode> previousLevel = mapLevels[level - 1];

            foreach (var node in currentLevel)
            {
                if (node.Type == NodeType.Boss) continue;

                if (node.ConnectedNodes.Count == 0)
                {
                    var unconnectedPrevNodes = previousLevel
                        .Where(prevNode => !node.ConnectedNodes.Contains(prevNode))
                        .Where(prevNode => Vector2.Distance(node.Position, prevNode.Position) <= maxConnectionDistance)
                        .ToList();
                    
                    if (unconnectedPrevNodes.Count == 0)
                    {
                        unconnectedPrevNodes = previousLevel
                            .Where(prevNode => !node.ConnectedNodes.Contains(prevNode))
                            .ToList();
                    }
                    
                    if (unconnectedPrevNodes.Count > 0)
                    {
                        var closestNode = unconnectedPrevNodes
                            .OrderBy(n => Vector2.Distance(node.Position, n.Position))
                            .First();
                        
                        node.ConnectedNodes.Add(closestNode);
                        closestNode.ConnectedNodes.Add(node);
                    }
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
        line.material = connectionMaterial;
        line.startWidth = connectionWidth;
        line.endWidth = connectionWidth;
        line.positionCount = 2;
        line.useWorldSpace = true;
        line.sortingOrder = -1;

        line.SetPosition(0, startPos);
        line.SetPosition(1, endPos);
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
            if (child.gameObject != currentPlayerMarker)
            {
                childrenToDestroy.Add(child.gameObject);
            }
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
}