using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using MapSystem;
using System.IO;

[System.Serializable]
public struct NodeTypeSettings
{
    public NodeType type;
    public int minCount;
    public int maxCount;
    public GameObject prefab;
    public int minLevel;
    [Tooltip("Y rotation for this node type")]
    public float yRotation;
    [Tooltip("Y offset for this node type")]
    public float yOffset;
}

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance { get; private set; }
    private const float PLAYER_MARKER_Y_OFFSET = 0.2f;
        public static bool ForceFreshGeneration = false;
    
    [Header("Node Type Configuration")]
    public NodeTypeSettings[] nodeTypes;

    [Header("Start Node Configuration")]
    public GameObject startNodePrefab;
    [Tooltip("Y rotation for start node")]
    public float startNodeYRotation = 0f;

    [Header("Player Configuration")]
    public GameObject playerMarkerPrefab;
    
    [HideInInspector]
    public GameObject currentPlayerMarker;

    [Header("Boss Configuration")]
    public GameObject bossPrefab;
    [Tooltip("Y rotation for boss node")]
    public float bossNodeYRotation = 0f;
    [Tooltip("Y offset for boss node")]
    public float bossYOffset = 0f;
    [Tooltip("Distance multiplier for boss node")]
    public float bossDistanceMultiplier = 1.5f;

    [Header("Map Settings")]
    public int numberOfLevels = 8;
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
    [Tooltip("Y offset for connection lines (lower = lines are drawn lower)")]
    public float connectionYOffset = 0.1f;
    [Range(4, 64)]
    [Tooltip("Number of segments used to draw curved connections (higher = smoother but more cost)")]
    public int connectionSegments = 24;
    [Range(0f, 1f)]
    [Tooltip("Curvature factor for Bezier connections (0 = straight line, 1 = very curved)")]
    public float connectionCurvature = 0.35f;
    [Header("Pulse Animation")]    
    [Tooltip("Speed of the subtle pulse animation for reachable edges")]
    public float pulseSpeed = 2.5f;
    [Tooltip("Width amplitude of the pulse animation, added on top of base width")]
    [Range(0f, 0.5f)] public float pulseWidthAmplitude = 0.02f;

    [Header("Node Connections")]
    public int minConnectionsPerNode = 1;
    public int maxConnectionsPerNode = 2;
    public float maxConnectionDistance = 8f;

    [Header("Camera")]
    public CameraController cameraController;
    [Header("UI / Info Panels")]
    public NodeInfoUI nodeInfoUI;

    public List<List<MapNode>> mapLevels = new List<List<MapNode>>();

    private Dictionary<NodeType, int> currentCounts = new Dictionary<NodeType, int>();
    private Dictionary<NodeType, NodeTypeSettings> settingsLookup = new Dictionary<NodeType, NodeTypeSettings>();
    private bool bossAssigned = false;

    [Header("References")]
    public MapManager mapManager;
    [SerializeField] private MapSaveManager mapSaveManager;
    
    [Header("Random Seed")]
    [Tooltip("If 0, uses random seed. Otherwise uses this seed for deterministic generation")]
    public int fixedSeed = 0;
    [HideInInspector]
    public int lastUsedSeed;

    private Transform startNodeTransform;
    private Transform bossNodeTransform;
    private Dictionary<Vector3, Transform> nodeTransformCache = new Dictionary<Vector3, Transform>();
    
    private List<NodeType> _availableTypesBuffer = new List<NodeType>();
    private List<int> _shuffleBuffer = new List<int>();
    private HashSet<(MapNode, MapNode)> _edgeSet = new HashSet<(MapNode, MapNode)>();
    private Dictionary<(Vector2, Vector2), float> _distanceCache = new Dictionary<(Vector2, Vector2), float>();
    private List<GameObject> _instantiatedNodes = new List<GameObject>();

    [Header("Debug")]
    [Tooltip("If true, extra debug logs will be printed during generation.")]
    public bool debugLogs = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[MapGenerator] Duplicate instance found, destroying this one.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        InitializeSettings();
        
        bool hasSave = mapSaveManager != null && mapSaveManager.HasSavedMap();
        if (ForceFreshGeneration)
        {
            if (debugLogs) Debug.Log("[MapGenerator] ForceFreshGeneration flag set -> ignoring save & using new random seed.");
            if (mapSaveManager != null)
            {
                mapSaveManager.DeleteSavedMap();
                if (debugLogs) Debug.Log("[MapGenerator] Deleted existing saved map for fresh generation.");
            }
            fixedSeed = 0;
            ForceFreshGeneration = false;
            GenerateMap();
            return;
        }

        if (hasSave)
        {
            Debug.Log("Loading saved map.");
            mapSaveManager.LoadSavedMap();
        }
        else
        {
            Debug.Log("Generating new map.");
            GenerateMap();
        }
    }

    public void NewGameFresh()
    {
        if (mapSaveManager != null)
        {
            mapSaveManager.DeleteSavedMap();
            if (debugLogs) Debug.Log("[MapGenerator] NewGameFresh: deleted saved map.");
        }
        fixedSeed = 0;
        GenerateMap();
        TrySaveJsonMap();
    }

    void OnValidate()
    {
        if (nodeTypes == null) return;

        var seen = new HashSet<NodeType>();
        for (int i = 0; i < nodeTypes.Length; i++)
        {
            var nt = nodeTypes[i];
            if (nt.minCount < 0) nt.minCount = 0;
            if (nt.maxCount < nt.minCount) nt.maxCount = nt.minCount;
            if (nt.minLevel < 0) nt.minLevel = 0;
            if (!seen.Add(nt.type))
            {
                Debug.LogWarning($"[MapGenerator] Duplicate NodeType in configuration: {nt.type}");
            }
            nodeTypes[i] = nt;
        }
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
        if (fixedSeed != 0)
        {
            Random.InitState(fixedSeed);
            lastUsedSeed = fixedSeed;
        }
        else
        {
            lastUsedSeed = (int)System.DateTime.Now.Ticks;
            Random.InitState(lastUsedSeed);
        }
        if (debugLogs)
            Debug.Log($"[MapGenerator] Generating map (seed={lastUsedSeed}).");

        ClearOldNodes();
        ResetMapState();
        GenerateMapStructure();
        AdjustBossNodePosition();
        ConnectNodes();
        InstantiateMapVisuals();
        SpawnPlayerMarker();
        DrawConnections();
        SetupCameraBounds();
        if (mapManager != null)
            mapManager.CalculateMaxMapTime();
    }

    public void ResetMapState()
    {
        bossAssigned = false;
        mapLevels.Clear();
        startNodeTransform = null;
        bossNodeTransform = null;
        nodeTransformCache.Clear();
        _edgeSet.Clear();
        _distanceCache.Clear();
        _availableTypesBuffer.Clear();
        _shuffleBuffer.Clear();
        foreach (var type in currentCounts.Keys.ToList())
        {
            currentCounts[type] = 0;
        }
    }

    public void GenerateMapStructure()
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
                _availableTypesBuffer.Clear();
                foreach (var kvp in settingsLookup)
                {
                    NodeType type = kvp.Key;
                    NodeTypeSettings settings = kvp.Value;
                    if (level < settings.minLevel)
                        continue;
                    if (currentCounts[type] < settings.maxCount)
                        _availableTypesBuffer.Add(type);
                }

                if (i > 0 && currentLevelNodes.Count > 0)
                {
                    NodeType prevType = currentLevelNodes[i - 1].Type;
                    _availableTypesBuffer.Remove(prevType);
                }

                if (_availableTypesBuffer.Count == 0)
                {
                    foreach (var kvp in settingsLookup)
                    {
                        NodeType type = kvp.Key;
                        NodeTypeSettings settings = kvp.Value;
                        if (level < settings.minLevel)
                            continue;
                        if (currentCounts[type] < settings.maxCount)
                            _availableTypesBuffer.Add(type);
                    }
                }

                if (_availableTypesBuffer.Count == 0 && level != numberOfLevels - 1)
                    break;

                float baseX = i * horizontalSpacing;
                float baseY = -level * verticalSpacing;
                baseX += Random.Range(-horizontalRandomRange, horizontalRandomRange);
                baseY += Random.Range(-verticalRandomRange, verticalRandomRange);
                float totalWidth = (nodesCount - 1) * horizontalSpacing;
                float offsetX = -totalWidth / 2f;

                var chosenType = GetRandomNodeType(level, _availableTypesBuffer);
                MapNode node = new MapNode
                {
                    Type = chosenType,
                    Position = new Vector2(baseX + offsetX, baseY),
                    EnemyVariant = (chosenType == NodeType.Enemy)
                        ? (Random.value < 0.5f ? MapSystem.EnemyVariant.Griffon : MapSystem.EnemyVariant.Satyr)
                        : MapSystem.EnemyVariant.None,
                    EliteVariant = (chosenType == NodeType.Elite)
                        ? (Random.value < 0.5f ? MapSystem.EliteVariant.Minotaur : MapSystem.EliteVariant.Arachne)
                        : MapSystem.EliteVariant.None
                };
                currentLevelNodes.Add(node);
            }
            mapLevels.Add(currentLevelNodes);
        }
    }

    public void SetupCameraBounds()
    {
        if (cameraController != null)
            cameraController.SetBoundsFromMap(this);
    }

    [ContextMenu("Regenerate (same seed)")]
    public void RegenerateSameSeed()
    {
        if (lastUsedSeed == 0 && fixedSeed == 0)
        {
            if (debugLogs) Debug.LogWarning("[MapGenerator] No previous seed; generating new map instead.");
            GenerateMap();
            return;
        }
        int seed = fixedSeed != 0 ? fixedSeed : lastUsedSeed;
        Random.InitState(seed);
        if (debugLogs) Debug.Log($"[MapGenerator] Regenerating with same seed {seed}");
        ClearOldNodes();
        ResetMapState();
        GenerateMapStructure();
        AdjustBossNodePosition();
        ConnectNodes();
        InstantiateMapVisuals();
        SpawnPlayerMarker();
        DrawConnections();
        SetupCameraBounds();
        if (mapManager != null) mapManager.CalculateMaxMapTime();
    }

    [ContextMenu("Regenerate (new seed)")]
    public void RegenerateNewSeed()
    {
        fixedSeed = 0;
        if (debugLogs) Debug.Log("[MapGenerator] Regenerating with new random seed");
        GenerateMap();
    }

    public void InstantiateMapVisuals()
    {
        EnsureEliteVariantsDistinct();
        for (int level = 0; level < mapLevels.Count; level++)
        {
            foreach (var node in mapLevels[level])
            {
                GameObject prefab = null;
                float yRotation = 0f;
                
                if (level == 0 && startNodePrefab != null && node.Type == NodeType.Start)
                {
                    prefab = startNodePrefab;
                    yRotation = startNodeYRotation;
                }
                else if (node.Type == NodeType.Boss)
                {
                    prefab = bossPrefab;
                    yRotation = bossNodeYRotation;
                }
                else
                {
                    prefab = GetPrefabForNodeType(node.Type);
                    if (settingsLookup.ContainsKey(node.Type))
                    {
                        yRotation = settingsLookup[node.Type].yRotation;
                    }
                }

                if (prefab != null)
                {
                    float nodeYOffset = 0f;
                    if (node.Type == NodeType.Boss)
                    {
                        nodeYOffset = bossYOffset;
                    }
                    else if (settingsLookup.ContainsKey(node.Type))
                    {
                        nodeYOffset = settingsLookup[node.Type].yOffset;
                    }
                    Vector3 pos3D = new Vector3(node.Position.x, nodeYOffset, node.Position.y);
                    Quaternion rotation = Quaternion.Euler(0, yRotation, 0);
                    GameObject instantiatedNode = Instantiate(prefab, pos3D, rotation, transform);
                    
                    _instantiatedNodes.Add(instantiatedNode);

                    nodeTransformCache[pos3D] = instantiatedNode.transform;

                    if (node.Type == NodeType.Start)
                        startNodeTransform = instantiatedNode.transform;
                    else if (node.Type == NodeType.Boss)
                        bossNodeTransform = instantiatedNode.transform;

                    var nodeHandler = instantiatedNode.GetComponent<NodeClickHandler>();
                    if (nodeHandler != null)
                    {
                        nodeHandler.nodeType = node.Type;
                        nodeHandler.mapNode = node;
                        if (string.IsNullOrEmpty(node.UniqueId))
                        {
                            string guid = System.Guid.NewGuid().ToString("N");
                            node.UniqueId = guid;
                        }
                        nodeHandler.enemyVariant = node.EnemyVariant;
                        nodeHandler.eliteVariant = node.EliteVariant;
                        nodeHandler.InjectDependencies(this, mapManager, cameraController, nodeInfoUI, mapSaveManager);
                    }
                }
            }
        }
    }

    private void EnsureEliteVariantsDistinct()
    {
        if (mapLevels == null || mapLevels.Count == 0) return;
        var elites = new System.Collections.Generic.List<MapNode>();
        for (int l = 0; l < mapLevels.Count; l++)
        {
            var level = mapLevels[l];
            if (level == null) continue;
            for (int i = 0; i < level.Count; i++)
            {
                var n = level[i];
                if (n != null && n.Type == NodeType.Elite)
                {
                    elites.Add(n);
                }
            }
        }
        if (elites.Count < 2) return;

        elites[0].EliteVariant = MapSystem.EliteVariant.Minotaur;
        elites[1].EliteVariant = MapSystem.EliteVariant.Arachne;

        for (int i = 2; i < elites.Count; i++)
        {
            elites[i].EliteVariant = (i % 2 == 0) ? MapSystem.EliteVariant.Minotaur : MapSystem.EliteVariant.Arachne;
        }
    }

    void SpawnPlayerMarker()
    {
        if (playerMarkerPrefab == null)
        {
            Debug.LogWarning("MapGenerator: playerMarkerPrefab is null");
            return;
        }
        
        if (mapLevels == null || mapLevels.Count == 0)
        {
            Debug.LogWarning("MapGenerator: mapLevels is null or empty");
            return;
        }

        if (mapLevels.Count > 0 && mapLevels[0] != null && mapLevels[0].Count > 0)
        {
            MapNode startNode = mapLevels[0][0];
            if (startNode == null)
            {
                Debug.LogWarning("MapGenerator: Start node is null");
                return;
            }
            
            Vector3 playerPos = new Vector3(startNode.Position.x, PLAYER_MARKER_Y_OFFSET, startNode.Position.y);
            
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
            
            Transform startTransform = GetStartNodeTransform();
            if (startTransform != null)
            {
                NodeClickHandler.currentPlayerNode = startTransform;
            }
            else
            {
                Debug.LogWarning("MapGenerator: Could not find start node transform");
            }
            
            NodeClickHandler.currentPlayerMapNode = startNode;
        }
        else
        {
            Debug.LogWarning("MapGenerator: First level has no nodes");
        }
    }

    Transform GetStartNodeTransform()
    {
        if (startNodeTransform != null)
            return startNodeTransform;
            
        if (mapLevels.Count > 0 && mapLevels[0] != null && mapLevels[0].Count > 0)
        {
            float startNodeYOffset = 0f;
            if (settingsLookup.ContainsKey(NodeType.Start))
                startNodeYOffset = settingsLookup[NodeType.Start].yOffset;
                
            Vector3 expectedPos = new Vector3(mapLevels[0][0].Position.x, startNodeYOffset, mapLevels[0][0].Position.y);
            
            if (nodeTransformCache.TryGetValue(expectedPos, out Transform cachedTransform))
            {
                startNodeTransform = cachedTransform;
                return startNodeTransform;
            }
        }
        
        return null;
    }

    public Transform GetClosestNodeTransform2D(Vector2 target)
    {
        Transform best = null;
        float bestDist = float.MaxValue;
        for (int level = 0; level < mapLevels.Count; level++)
        {
            foreach (var node in mapLevels[level])
            {
                float yOff = 0f;
                if (node.Type == NodeType.Boss) yOff = bossYOffset;
                else if (settingsLookup.ContainsKey(node.Type)) yOff = settingsLookup[node.Type].yOffset;
                Vector3 pos3D = new Vector3(node.Position.x, yOff, node.Position.y);
                if (!nodeTransformCache.TryGetValue(pos3D, out var tf) || tf == null) continue;
                Vector2 node2D = new Vector2(pos3D.x, pos3D.z);
                
                var cacheKey = GetDistanceCacheKey(node2D, target);
                if (!_distanceCache.TryGetValue(cacheKey, out float d))
                {
                    d = Vector2.Distance(node2D, target);
                    _distanceCache[cacheKey] = d;
                }
                
                if (d < bestDist)
                {
                    bestDist = d;
                    best = tf;
                }
            }
        }
        return best;
    }

    private (Vector2, Vector2) GetDistanceCacheKey(Vector2 a, Vector2 b)
    {
        return (a.x < b.x || (a.x == b.x && a.y <= b.y)) ? (a, b) : (b, a);
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
            List<MapNode> curr = mapLevels[level];
            List<MapNode> next = mapLevels[level + 1];
            if (curr.Count == 0 || next.Count == 0) continue;

            List<int> currOrder = new List<int>(curr.Count);
            for (int i = 0; i < curr.Count; i++) currOrder.Add(i);
            currOrder.Sort((a, b) => curr[a].Position.x.CompareTo(curr[b].Position.x));

            List<int> nextOrder = new List<int>(next.Count);
            for (int j = 0; j < next.Count; j++) nextOrder.Add(j);
            nextOrder.Sort((a, b) => next[a].Position.x.CompareTo(next[b].Position.x));

            int[] degCurr = new int[curr.Count];
            int[] degNext = new int[next.Count];

            List<(Vector2, Vector2)> edges = new List<(Vector2, Vector2)>(curr.Count * 2);

            for (int idx = 0; idx < nextOrder.Count; idx++)
            {
                int j = nextOrder[idx];
                Vector2 targetPos = next[j].Position;
                float bestDist = float.MaxValue;
                int bestI = -1;

                for (int ii = 0; ii < currOrder.Count; ii++)
                {
                    int i = currOrder[ii];
                    if (degCurr[i] >= maxConnectionsPerNode) continue;
                    
                    var cacheKey = GetDistanceCacheKey(curr[i].Position, targetPos);
                    if (!_distanceCache.TryGetValue(cacheKey, out float d))
                    {
                        d = Vector2.Distance(curr[i].Position, targetPos);
                        _distanceCache[cacheKey] = d;
                    }
                    
                    if (maxConnectionDistance > 0 && d > maxConnectionDistance) continue;
                    if (d < bestDist)
                    {
                        bestDist = d;
                        bestI = i;
                    }
                }

                if (bestI == -1)
                {
                    for (int ii = 0; ii < currOrder.Count; ii++)
                    {
                        int i = currOrder[ii];
                        if (degCurr[i] >= maxConnectionsPerNode) continue;
                        
                        var cacheKey = GetDistanceCacheKey(curr[i].Position, targetPos);
                        if (!_distanceCache.TryGetValue(cacheKey, out float d))
                        {
                            d = Vector2.Distance(curr[i].Position, targetPos);
                            _distanceCache[cacheKey] = d;
                        }
                        
                        if (d < bestDist)
                        {
                            bestDist = d;
                            bestI = i;
                        }
                    }
                }

                if (bestI != -1)
                {
                    TryAddConnection(curr[bestI], next[j], degCurr, degNext, bestI, j, edges);
                }
            }

            for (int ci = 0; ci < currOrder.Count; ci++)
            {
                int i = currOrder[ci];
                while (degCurr[i] < minConnectionsPerNode)
                {
                    int bestJ = -1;
                    float bestDist2 = float.MaxValue;
                    for (int nj = 0; nj < next.Count; nj++)
                    {
                        if (curr[i].ConnectedNodes.Contains(next[nj])) continue;
                        
                        var cacheKey = GetDistanceCacheKey(curr[i].Position, next[nj].Position);
                        if (!_distanceCache.TryGetValue(cacheKey, out float d))
                        {
                            d = Vector2.Distance(curr[i].Position, next[nj].Position);
                            _distanceCache[cacheKey] = d;
                        }
                        
                        if (maxConnectionDistance > 0 && d > maxConnectionDistance) continue;
                        if (WouldCross(curr[i].Position, next[nj].Position, edges)) continue;
                        if (d < bestDist2)
                        {
                            bestDist2 = d;
                            bestJ = nj;
                        }
                    }

                    if (bestJ == -1)
                    {
                        for (int nj = 0; nj < next.Count; nj++)
                        {
                            if (degNext[nj] >= Mathf.Max(1, maxConnectionsPerNode)) continue;
                            if (curr[i].ConnectedNodes.Contains(next[nj])) continue;
                            
                            var cacheKey = GetDistanceCacheKey(curr[i].Position, next[nj].Position);
                            if (!_distanceCache.TryGetValue(cacheKey, out float d))
                            {
                                d = Vector2.Distance(curr[i].Position, next[nj].Position);
                                _distanceCache[cacheKey] = d;
                            }
                            
                            if (maxConnectionDistance > 0 && d > maxConnectionDistance) continue;
                            if (d < bestDist2)
                            {
                                bestDist2 = d;
                                bestJ = nj;
                            }
                        }
                    }

                    if (bestJ == -1) break;
                    TryAddConnection(curr[i], next[bestJ], degCurr, degNext, i, bestJ, edges);
                }
            }
        }

        EnsureAllNodesConnected();
    }

    bool WouldCross(Vector2 a, Vector2 b, List<(Vector2, Vector2)> edges)
    {
        float minX_ab = Mathf.Min(a.x, b.x);
        float maxX_ab = Mathf.Max(a.x, b.x);
        float minY_ab = Mathf.Min(a.y, b.y);
        float maxY_ab = Mathf.Max(a.y, b.y);
        
        for (int k = 0; k < edges.Count; k++)
        {
            var (c, d) = edges[k];
            
            float minX_cd = Mathf.Min(c.x, d.x);
            float maxX_cd = Mathf.Max(c.x, d.x);
            float minY_cd = Mathf.Min(c.y, d.y);
            float maxY_cd = Mathf.Max(c.y, d.y);
            
            if (maxX_ab < minX_cd || minX_ab > maxX_cd || maxY_ab < minY_cd || minY_ab > maxY_cd)
                continue;
            
            if (SegmentsCross(a, b, c, d)) return true;
        }
        return false;
    }

    bool SegmentsCross(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2)
    {
        if ((p1 == p2 && q1 == q2) || (p1 == q2 && q1 == p2)) return false;

        int o1 = Orientation(p1, q1, p2);
        int o2 = Orientation(p1, q1, q2);
        int o3 = Orientation(p2, q2, p1);
        int o4 = Orientation(p2, q2, q1);

        if (o1 != o2 && o3 != o4) return true;

        if (o1 == 0 && OnSegment(p1, p2, q1)) return true;
        if (o2 == 0 && OnSegment(p1, q2, q1)) return true;
        if (o3 == 0 && OnSegment(p2, p1, q2)) return true;
        if (o4 == 0 && OnSegment(p2, q1, q2)) return true;

        return false;
    }

    int Orientation(Vector2 p, Vector2 q, Vector2 r)
    {
        float val = (q.y - p.y) * (r.x - q.x) - (q.x - p.x) * (r.y - q.y);
        if (Mathf.Approximately(val, 0f)) return 0;
        return (val > 0) ? 1 : 2;
    }

    bool OnSegment(Vector2 p, Vector2 q, Vector2 r)
    {
        return q.x <= Mathf.Max(p.x, r.x) + 1e-4f && q.x + 1e-4f >= Mathf.Min(p.x, r.x)
            && q.y <= Mathf.Max(p.y, r.y) + 1e-4f && q.y + 1e-4f >= Mathf.Min(p.y, r.y);
    }

    void TryAddConnection(MapNode from, MapNode to, int[] degCurr, int[] degNext, int iIdx, int jIdx, List<(Vector2, Vector2)> edges)
    {
        if (!from.ConnectedNodes.Contains(to)) from.ConnectedNodes.Add(to);
        if (!to.ConnectedNodes.Contains(from)) to.ConnectedNodes.Add(from);
        degCurr[iIdx]++;
        degNext[jIdx]++;
        edges.Add((from.Position, to.Position));
    }


    void AdjustBossNodePosition()
    {
        if (bossDistanceMultiplier <= 1.0f) return;

        int bossLevel = numberOfLevels - 1;
        if (bossLevel >= mapLevels.Count) return;

        List<MapNode> bossNodes = mapLevels[bossLevel];
        if (bossNodes.Count == 0) return;

        foreach (var bossNode in bossNodes)
        {
            if (bossNode.Type == NodeType.Boss)
            {
                Vector2 centerOfMap = Vector2.zero;
                
                int totalNodes = 0;
                Vector2 sumPositions = Vector2.zero;
                for (int level = 0; level < mapLevels.Count - 1; level++)
                {
                    foreach (var node in mapLevels[level])
                    {
                        sumPositions += node.Position;
                        totalNodes++;
                    }
                }
                
                if (totalNodes > 0)
                {
                    centerOfMap = sumPositions / totalNodes;
                }

                Vector2 directionFromCenter = (bossNode.Position - centerOfMap).normalized;
                
                float additionalDistance = verticalSpacing * (bossDistanceMultiplier - 1.0f);
                bossNode.Position += directionFromCenter * additionalDistance;
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

    public void DrawConnections()
    {
        HashSet<ulong> drawnConnections = new HashSet<ulong>();

        foreach (var level in mapLevels)
        {
            foreach (var node in level)
            {
                Transform startTf = GetTransformForMapNode(node);
                if (startTf == null) continue;

                foreach (var connectedNode in node.ConnectedNodes)
                {
                    Transform endTf = GetTransformForMapNode(connectedNode);
                    if (endTf == null) continue;

                    int a = startTf.GetInstanceID();
                    int b = endTf.GetInstanceID();
                    ulong key = a < b ? ((ulong)(uint)a << 32) | (uint)b : ((ulong)(uint)b << 32) | (uint)a;
                    if (drawnConnections.Contains(key)) continue;
                    drawnConnections.Add(key);

                    CreateCurvedConnection(startTf, endTf);
                }
            }
        }
    }

    void CreateCurvedConnection(Transform startTf, Transform endTf)
    {
        string name = $"Connection_{startTf.name}_to_{endTf.name}";
        GameObject lineObj = new GameObject(name);
        lineObj.transform.SetParent(transform);

        var curved = lineObj.AddComponent<CurvedConnectionLine>();
        curved.Initialize(new CurvedConnectionLine.Config
        {
            start = startTf,
            end = endTf,
            yOffset = connectionYOffset,
            width = connectionWidth,
            segments = Mathf.Max(4, connectionSegments),
            curvature = connectionCurvature,
            material = connectionMaterial,
            pulseSpeed = pulseSpeed,
            pulseWidthAmp = pulseWidthAmplitude,
            pulseOnlyWhenReachable = true,
        });
    }

    Transform GetTransformForMapNode(MapNode node)
    {
        float nodeYOffset = 0f;
        if (node.Type == NodeType.Boss) nodeYOffset = bossYOffset;
        else if (settingsLookup.ContainsKey(node.Type)) nodeYOffset = settingsLookup[node.Type].yOffset;
        Vector3 pos3D = new Vector3(node.Position.x, nodeYOffset, node.Position.y);
        nodeTransformCache.TryGetValue(pos3D, out var tf);
        return tf;
    }

    GameObject GetPrefabForNodeType(NodeType type)
    {
        if (type == NodeType.Boss)
            return bossPrefab;

        return settingsLookup.ContainsKey(type) ? settingsLookup[type].prefab : null;
    }

    void ClearOldNodes()
    {
        ClearTrackedNodes();
        ClearUnparentedChildren();
        nodeTransformCache.Clear();
        startNodeTransform = null;
        bossNodeTransform = null;
    }

    private void ClearTrackedNodes()
    {
        for (int i = _instantiatedNodes.Count - 1; i >= 0; i--)
        {
            var node = _instantiatedNodes[i];
            if (node != null && node != currentPlayerMarker)
            {
                if (Application.isPlaying)
                    Destroy(node);
                else
                    DestroyImmediate(node);
            }
        }
        _instantiatedNodes.Clear();
    }

    private void ClearUnparentedChildren()
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

    public JsonMapSave BuildJsonSave()
    {
        var save = new JsonMapSave();
        save.version = "1.0";
        save.seed = lastUsedSeed;
        if (MapManager.Instance != null)
        {
            save.maxMapTime = MapManager.Instance.maxMapTime;
            save.currentMapTime = MapManager.Instance.currentMapTime;
        }
        
        if (NodeClickHandler.currentPlayerNode != null)
        {
            var currentHandler = NodeClickHandler.currentPlayerNode.GetComponent<NodeClickHandler>();
            if (currentHandler != null && currentHandler.mapNode != null && !string.IsNullOrEmpty(currentHandler.mapNode.UniqueId))
            {
                save.currentPlayerNodeGuid = currentHandler.mapNode.UniqueId;
            }
            else if (NodeClickHandler.currentPlayerMapNode != null && !string.IsNullOrEmpty(NodeClickHandler.currentPlayerMapNode.UniqueId))
            {
                save.currentPlayerNodeGuid = NodeClickHandler.currentPlayerMapNode.UniqueId;
            }
        }

        for (int level = 0; level < mapLevels.Count; level++)
        {
            foreach (var node in mapLevels[level])
            {
                if (node == null) continue;
                if (string.IsNullOrEmpty(node.UniqueId))
                    node.UniqueId = System.Guid.NewGuid().ToString("N");
                var rec = new JsonNodeRecord
                {
                    guid = node.UniqueId,
                    type = node.Type,
                    position = node.Position,
                    levelIndex = level,
                    cleared = NodeClickHandler.IsCombatNode(node.Type) && NodeClickHandler.IsGuidCleared(node.UniqueId),
                    enemyVariant = node.EnemyVariant,
                    eliteVariant = node.EliteVariant
                };
                save.nodes.Add(rec);
                if (node.ConnectedNodes != null)
                {
                    foreach (var other in node.ConnectedNodes)
                    {
                        if (other == null) continue;
                        if (string.IsNullOrEmpty(other.UniqueId))
                            other.UniqueId = System.Guid.NewGuid().ToString("N");
                        if (string.Compare(rec.guid, other.UniqueId, System.StringComparison.Ordinal) < 0)
                        {
                            save.edges.Add(new JsonEdgeRecord { fromGuid = rec.guid, toGuid = other.UniqueId });
                        }
                    }
                }
            }
        }
        return save;
    }

    public void TrySaveJsonMap()
    {
        try
        {
            var data = BuildJsonSave();
            JsonSaveManager.Save(data);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[MapGenerator] TrySaveJsonMap failed: {ex}");
        }
    }

    public void TryLoadLatestJsonAndApply()
    {
        string path = JsonSaveManager.GetLatestSavePath();
        if (string.IsNullOrEmpty(path))
        {
            Debug.Log("[MapGenerator] No JSON save found.");
            return;
        }
        var data = JsonSaveManager.Load(path);
        if (data == null)
        {
            Debug.LogWarning("[MapGenerator] Failed to load JSON save.");
            return;
        }
        ApplyJsonSave(data);
    }

    public void ApplyJsonSave(JsonMapSave data)
    {
        if (data == null)
        {
            Debug.LogWarning("[MapGenerator] ApplyJsonSave: data null");
            return;
        }
        var levelsDict = new Dictionary<int, List<MapNode>>();
        foreach (var rec in data.nodes)
        {
            if (!levelsDict.TryGetValue(rec.levelIndex, out var list))
            {
                list = new List<MapNode>();
                levelsDict[rec.levelIndex] = list;
            }
            var mn = new MapNode
            {
                Type = rec.type,
                Position = rec.position,
                UniqueId = rec.guid,
                EnemyVariant = rec.enemyVariant,
                EliteVariant = rec.eliteVariant
            };
            list.Add(mn);
        }
        var ordered = levelsDict.Keys.OrderBy(k => k).ToList();
        mapLevels.Clear();
        foreach (var k in ordered)
            mapLevels.Add(levelsDict[k]);

        var guidToNode = mapLevels.SelectMany(l => l).ToDictionary(n => n.UniqueId, n => n);
        foreach (var e in data.edges)
        {
            if (guidToNode.TryGetValue(e.fromGuid, out var a) && guidToNode.TryGetValue(e.toGuid, out var b))
            {
                if (a.ConnectedNodes == null) a.ConnectedNodes = new List<MapNode>();
                if (b.ConnectedNodes == null) b.ConnectedNodes = new List<MapNode>();
                if (!a.ConnectedNodes.Contains(b)) a.ConnectedNodes.Add(b);
                if (!b.ConnectedNodes.Contains(a)) b.ConnectedNodes.Add(a);
            }
        }

        if (MapManager.Instance != null)
        {
            MapManager.Instance.maxMapTime = data.maxMapTime;
            MapManager.Instance.currentMapTime = data.currentMapTime;
        }

        ClearOldNodes();
        InstantiateMapVisuals();
        SpawnPlayerMarker();
        DrawConnections();
        SetupCameraBounds();
        
        if (!string.IsNullOrEmpty(data.currentPlayerNodeGuid))
        {
            RestorePlayerPosition(data.currentPlayerNodeGuid);
        }
    }
    
    private void RestorePlayerPosition(string nodeGuid)
    {
        if (string.IsNullOrEmpty(nodeGuid))
        {
            Debug.LogWarning("[MapGenerator] RestorePlayerPosition: nodeGuid is null or empty");
            return;
        }
        
        MapNode targetMapNode = null;
        for (int level = 0; level < mapLevels.Count; level++)
        {
            foreach (var node in mapLevels[level])
            {
                if (node != null && node.UniqueId == nodeGuid)
                {
                    targetMapNode = node;
                    break;
                }
            }
            if (targetMapNode != null) break;
        }
        
        if (targetMapNode == null)
        {
            Debug.LogWarning($"[MapGenerator] Could not find node with GUID: {nodeGuid}");
            return;
        }
        
        float yOff = 0f;
        if (targetMapNode.Type == NodeType.Boss)
            yOff = bossYOffset;
        else if (settingsLookup.ContainsKey(targetMapNode.Type))
            yOff = settingsLookup[targetMapNode.Type].yOffset;
        
        Vector3 pos3D = new Vector3(targetMapNode.Position.x, yOff, targetMapNode.Position.y);
        
        if (nodeTransformCache.TryGetValue(pos3D, out Transform nodeTransform))
        {
            NodeClickHandler.currentPlayerNode = nodeTransform;
            NodeClickHandler.currentPlayerMapNode = targetMapNode;
            
            if (currentPlayerMarker != null)
            {
                Vector3 playerPos = new Vector3(targetMapNode.Position.x, PLAYER_MARKER_Y_OFFSET, targetMapNode.Position.y);
                currentPlayerMarker.transform.position = playerPos;
            }
            
            if (debugLogs)
                Debug.Log($"[MapGenerator] Restored player to node: {targetMapNode.Type} at {targetMapNode.Position}");
        }
        else
        {
            Debug.LogWarning($"[MapGenerator] Could not find transform for node at position: {pos3D}");
        }
    }
    
    public Transform GetBossNodeTransform()
    {
        if (bossNodeTransform != null)
            return bossNodeTransform;
            
        if (mapLevels.Count == 0) return null;
        
        var lastLevel = mapLevels[mapLevels.Count - 1];
        foreach (var node in lastLevel)
        {
            if (node.Type == NodeType.Boss)
            {
                Vector3 nodePos3D = new Vector3(node.Position.x, bossYOffset, node.Position.y);
                
                if (nodeTransformCache.TryGetValue(nodePos3D, out Transform cachedTransform))
                {
                    bossNodeTransform = cachedTransform;
                    return bossNodeTransform;
                }
            }
        }
        
        Debug.LogWarning("[MapGenerator] Boss node not found!");
        return null;
    }

    private MapNode GetBossMapNode()
    {
        if (mapLevels == null || mapLevels.Count == 0) return null;
        var last = mapLevels[mapLevels.Count - 1];
        for (int i = 0; i < last.Count; i++)
        {
            if (last[i].Type == NodeType.Boss) return last[i];
        }
        return null;
    }

}