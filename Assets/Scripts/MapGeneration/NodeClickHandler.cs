using UnityEngine;
using UnityEngine.SceneManagement;
using MapSystem;
using Systems.SceneManagement;
using System.Collections.Generic;

public class NodeClickHandler : MonoBehaviour
{
    [Header("Scene To Load")]
    public string sceneToLoad = "";
    
    [Header("Player Tracking")]
    public static Transform currentPlayerNode;
    public static GameObject playerMarker;
    public GameObject playerMarkerPrefab;
    public static MapNode currentPlayerMapNode;
    
    [Header("References")]
    public MapGenerator mapGenerator; //TODO: singleton
    public MapManager mapManager; //TODO: singleton
    [Header("Optional (injected)")]
    public CameraController cameraController;
    public NodeInfoUI nodeInfoUI;
    public MapSaveManager mapSaveManager;

    [Header("Player Movement")]
    public float moveSpeed = 3f;
    public float jumpHeight = 1f;
    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve jumpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Node Info")]
    public NodeType nodeType = NodeType.Enemy;
    [Tooltip("Backing MapNode for this instance, set by MapGenerator during instantiation")]
    public MapNode mapNode;
    
    private static bool isPlayerMoving = false; // TODO: do skryptu playera
    private bool _shopBlock = false;
    private bool _healBlock = false; // TODO: do skryptu nodea
    private bool _wasClearedAndDisabled = false; 

    private static readonly HashSet<NodeClickHandler> _clearedCombatNodes = new HashSet<NodeClickHandler>();
    public static NodeClickHandler lastEnteredCombatNode;
    private static readonly HashSet<string> _clearedCombatGuids = new HashSet<string>();
    private static string _lastEnteredCombatGuid;
    private static readonly HashSet<string> _clearedCombatKeys = new HashSet<string>();
    private static string _lastEnteredCombatKey;
    
    [Header("Debug")]
    [Tooltip("Enable verbose debug logs for movement / adjacency checks.")]
    public bool enableDebugLogs = false;
    
    public static bool forceDebugLogs = true;
    

    void Start()
    {
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }

        ConsumablesEventSystem.OnSceneBlock.AddListener(BlockScene);

        if (mapNode == null)
        {
            Debug.LogWarning($"[NodeClickHandler] mapNode reference is NULL on '{name}'. Movement adjacency will fail for this node.");
        }
        else
        {
            var guid = GetOrAssignNodeGuid(this);
            if (!string.IsNullOrEmpty(guid) && _clearedCombatGuids.Contains(guid))
            {
                _wasClearedAndDisabled = true;
                if (forceDebugLogs || enableDebugLogs)
                    Debug.Log($"[NodeClickHandler] Start: Node '{name}' already cleared by GUID {guid}. Disabling loads.");
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    ShowNodeInfo();
                }
            }
        }
    }

    private Vector3 mouseDownPosition;
    private float dragThreshold = 10f;

    void OnMouseDown()
    {
        mouseDownPosition = Input.mousePosition;
        
        if (Input.GetMouseButton(1))
        {
            ShowNodeInfo();
            return;
        }
    }
    
    void OnMouseUp()
    {
        float dragDistance = Vector3.Distance(Input.mousePosition, mouseDownPosition);
        
        if (dragDistance < dragThreshold)
        {
            bool allowed = CanMoveToThisNode();
            if (forceDebugLogs || enableDebugLogs)
            {
                Debug.Log($"[NodeClickHandler][CLICK] Attempt move from '{(currentPlayerNode? currentPlayerNode.name : "<none>")}' to '{name}' | allowed={allowed}");
            }
            if (allowed) MovePlayerHere();
        }
    }

    bool CanMoveToThisNode() // TODO: do managera mapy albo node
    {
        if (isPlayerMoving)
        { LogMoveBlock("Already moving"); return false; }
        
        if (currentPlayerNode == null) 
        { LogMoveInfo("No current player node (first placement)"); return true; }
        
    if (mapGenerator == null || mapGenerator.mapLevels == null) { LogMoveBlock("MapGenerator/mapLevels null"); return false; }
        
        if (mapManager != null && !mapManager.HasTimeLeft())
        {
            Debug.Log("[NodeClickHandler] No more time left");
            return false;
        }
        
    var currentHandler = currentPlayerNode.GetComponent<NodeClickHandler>();
    MapNode sourceMapNode = null;
    if (currentHandler != null && currentHandler.mapNode != null)
    {
        sourceMapNode = currentHandler.mapNode;
    }
    else
    {
        if (currentPlayerMapNode != null)
        {
            sourceMapNode = currentPlayerMapNode;
            if (enableDebugLogs) Debug.Log("[NodeClickHandler] Using cached currentPlayerMapNode fallback");
        }
        else if (mapGenerator != null && mapGenerator.mapLevels != null)
        {
            Vector2 currPos2D = new Vector2(currentPlayerNode.position.x, currentPlayerNode.position.z);
            const float EPS = 0.05f;
            foreach (var level in mapGenerator.mapLevels)
            {
                foreach (var n in level)
                {
                    if (Vector2.Distance(n.Position, currPos2D) <= EPS)
                    {
                        sourceMapNode = n;
                        currentPlayerMapNode = n;
                        if (enableDebugLogs) Debug.Log("[NodeClickHandler] Resolved current player MapNode by position search");
                        break;
                    }
                }
                if (sourceMapNode != null) break;
            }
        }
    }
    if (sourceMapNode == null) { LogMoveBlock("Cannot resolve current MapNode"); return false; }
    if (mapNode == null) { LogMoveBlock("Target mapNode null"); return false; }

    bool reachable = sourceMapNode.ConnectedNodes.Contains(mapNode);
    if (enableDebugLogs)
    {
        int currentConn = sourceMapNode.ConnectedNodes != null ? sourceMapNode.ConnectedNodes.Count : -1;
        Debug.Log($"[NodeClickHandler] Adjacency check: from '{currentPlayerNode.name}' -> '{name}' ({nodeType}) | currentConnections={currentConn} | reachable={reachable}");
        if (!reachable)
        {
            string list = string.Join(", ", sourceMapNode.ConnectedNodes.ConvertAll(n => n.Type + "@" + n.Position));
            Debug.Log($"[NodeClickHandler] ConnectedNodes of current: {list}");
        }
    }
    if (!reachable) LogMoveBlock("Target not in ConnectedNodes list of current node"); else LogMoveInfo("Reachable OK");
    return reachable;
    }

    void LogMoveBlock(string reason)
    {
        if (forceDebugLogs || enableDebugLogs)
            Debug.Log($"[NodeClickHandler][BLOCK] '{name}' reason={reason} current='{(currentPlayerNode? currentPlayerNode.name : "<none>")}'");
    }

    void LogMoveInfo(string msg)
    {
        if (forceDebugLogs || enableDebugLogs)
            Debug.Log($"[NodeClickHandler][INFO] '{name}' {msg}");
    }
    
    void MovePlayerHere()
    {
        if (playerMarker != null && currentPlayerNode != null)
        {
            StartCoroutine(SmoothMovePlayer());
        }
        else
        {
            MovePlayerInstant();
        }
    }
    
    System.Collections.IEnumerator SmoothMovePlayer()
    {
        isPlayerMoving = true;
        
        Vector3 fromPos = currentPlayerNode.position;
        Vector3 toPos = transform.position;

        if (mapManager != null)
        {
            Vector2 fromPos2D = new Vector2(fromPos.x, fromPos.z);
            Vector2 toPos2D = new Vector2(toPos.x, toPos.z);
            mapManager.TravelToNode(fromPos2D, toPos2D);
        }
        
        float duration = Vector3.Distance(fromPos, toPos) / moveSpeed;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            float horizontalT = moveCurve.Evaluate(t);
            Vector3 horizontalPos = Vector3.Lerp(fromPos, toPos, horizontalT);

            float jumpT = jumpCurve.Evaluate(t);
            float jumpOffset = Mathf.Sin(jumpT * Mathf.PI) * jumpHeight;

            Vector3 currentPos = horizontalPos;
            currentPos.y = horizontalPos.y + jumpOffset;
            
            playerMarker.transform.position = currentPos;

            Vector3 direction = (toPos - fromPos).normalized;
            if (direction != Vector3.zero)
            {
                float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                playerMarker.transform.rotation = Quaternion.Lerp(
                    playerMarker.transform.rotation,
                    Quaternion.Euler(0, angle, 0),
                    Time.deltaTime * 5f
                );
            }
            
            yield return null;
        }

        playerMarker.transform.position = toPos;
        currentPlayerNode = transform;
    currentPlayerMapNode = mapNode;
        if (mapSaveManager != null)
        {
            mapSaveManager.SaveCurrentMap();
        }
        isPlayerMoving = false;
        
        LoadScene();
    }
    
    void MovePlayerInstant()
    {
        Vector3 fromPos3D = Vector3.zero;
        Vector3 toPos3D = transform.position;
        bool hasFromPosition = false;
        
        if (currentPlayerNode != null && mapManager != null)
        {
            fromPos3D = currentPlayerNode.position;
            hasFromPosition = true;
            Vector2 fromPos = new Vector2(fromPos3D.x, fromPos3D.z);
            Vector2 toPos = new Vector2(toPos3D.x, toPos3D.z);
            mapManager.TravelToNode(fromPos, toPos);
        }
        
        currentPlayerNode = transform;
    currentPlayerMapNode = mapNode;
        
    if (playerMarker != null)
        {
            playerMarker.transform.position = toPos3D;
            
            if (hasFromPosition)
            {
                Vector3 direction = (toPos3D - fromPos3D).normalized;
                if (direction != Vector3.zero)
                {
                    float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                    playerMarker.transform.rotation = Quaternion.Euler(0, angle, 0);
                }
            }
            
            playerMarker.SetActive(true);
        }

        if (mapSaveManager != null)
        {
            mapSaveManager.SaveCurrentMap();
        }

        LoadScene();
    }

    void LoadScene() // TODO: do skryptu od ladowania scen
    {
        string myGuid = IsCombatNode(nodeType) ? GetOrAssignNodeGuid(this) : null;
        string myKey = IsCombatNode(nodeType) ? GetNodeKey(this) : null;
        if (_wasClearedAndDisabled)
        {
            if (forceDebugLogs || enableDebugLogs)
                Debug.Log($"[NodeClickHandler] Node '{name}' is disabled after clear. Staying on map.");
            return;
        }
        if (IsCombatNode(nodeType) &&
            (_clearedCombatNodes.Contains(this) ||
             (!string.IsNullOrEmpty(myGuid) && _clearedCombatGuids.Contains(myGuid)) ||
             (myKey != null && _clearedCombatKeys.Contains(myKey))))
        {
            Debug.Log($"[NodeClickHandler] Node '{name}' already cleared (guid={myGuid ?? "<none>"}, key={myKey ?? "<none>"}). Staying on map.");
            return;
        }

        if (!string.IsNullOrEmpty(sceneToLoad) && IsSceneBlocked(sceneToLoad))
        {
            if (IsCombatNode(nodeType))
            {
                CombatManager.ApplyNodeType(nodeType);
            }
            
            if (mapSaveManager != null)
            {
                mapSaveManager.SaveCurrentMap();
            }
            
            if (SceneLoader.Instance != null)
            {
                if (IsCombatNode(nodeType))
                {
                    lastEnteredCombatNode = this;
                    _lastEnteredCombatGuid = myGuid;
                    _lastEnteredCombatKey = myKey;
                    Debug.Log($"[NodeClickHandler] Entering combat from node '{name}' guid={_lastEnteredCombatGuid} key={_lastEnteredCombatKey}");
                    DisableNodeOnCombatEntry(_lastEnteredCombatGuid);
                }
                Debug.Log("Using additive scene activation for: " + sceneToLoad);
                SceneLoader.Instance.manager.ActivateScene(sceneToLoad);
                if (IsCombatNode(nodeType))
                {
                    CombatManager.ApplyNodeType(nodeType);
                }
                return;
            }
            
            Debug.Log("SceneLoader not found, using normal scene loading for: " + sceneToLoad);
            if (IsCombatNode(nodeType))
            {
                lastEnteredCombatNode = this;
                _lastEnteredCombatGuid = myGuid;
                _lastEnteredCombatKey = myKey;
                Debug.Log($"[NodeClickHandler] Entering combat (single load) from node '{name}' guid={_lastEnteredCombatGuid} key={_lastEnteredCombatKey}");
                DisableNodeOnCombatEntry(_lastEnteredCombatGuid);
            }
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    static bool IsCombatNode(NodeType t)
    {
        return t == NodeType.Enemy || t == NodeType.Elite || t == NodeType.Boss;
    }
    
    public void TriggerLoadScene()
    {
        LoadScene();
    }

    public static void MarkLastCombatNodeClearedIfAny()
    {
        bool any = false;
        if (!string.IsNullOrEmpty(_lastEnteredCombatGuid))
        {
            _clearedCombatGuids.Add(_lastEnteredCombatGuid);
            Debug.Log($"[NodeClickHandler] Marked GUID '{_lastEnteredCombatGuid}' as cleared");
            DisableNodesWithGuid(_lastEnteredCombatGuid);
            any = true;
        }
        if (!string.IsNullOrEmpty(_lastEnteredCombatKey))
        {
            _clearedCombatKeys.Add(_lastEnteredCombatKey);
            Debug.Log($"[NodeClickHandler] Marked key '{_lastEnteredCombatKey}' as cleared");
            any = true;
        }
        if (lastEnteredCombatNode != null)
        {
            _clearedCombatNodes.Add(lastEnteredCombatNode);
            Debug.Log($"[NodeClickHandler] Also marked instance '{lastEnteredCombatNode.name}' as cleared (legacy)");
            lastEnteredCombatNode.DisableNodeAfterClear();
            any = true;
        }
        if (!any)
        {
            Debug.LogWarning("[NodeClickHandler] No last entered combat node/key to mark as cleared");
        }
        lastEnteredCombatNode = null;
        _lastEnteredCombatGuid = null;
        _lastEnteredCombatKey = null;
    }

    public static void ClearClearedNodes()
    {
        _clearedCombatNodes.Clear();
        _clearedCombatGuids.Clear();
        _clearedCombatKeys.Clear();
        lastEnteredCombatNode = null;
        _lastEnteredCombatGuid = null;
        _lastEnteredCombatKey = null;
    }

    private static string GetNodeKey(NodeClickHandler h)
    {
        if (h != null)
        {
            if (h.mapNode != null)
            {
                var p = h.mapNode.Position;
                return $"{p.x:F3},{p.y:F3}";
            }
            var pos = h.transform.position;
            return $"{pos.x:F3},{pos.z:F3}";
        }
        return null;
    }

    private static string GetOrAssignNodeGuid(NodeClickHandler h)
    {
        if (h == null) return null;
        if (h.mapNode == null) return null;
        if (!string.IsNullOrEmpty(h.mapNode.UniqueId)) return h.mapNode.UniqueId;
        string guid = System.Guid.NewGuid().ToString("N");
        h.mapNode.UniqueId = guid;
        if (forceDebugLogs || h.enableDebugLogs)
            Debug.Log($"[NodeClickHandler] Assigned GUID '{guid}' to node at {h.mapNode.Position}");
        return guid;
    }

    private void DisableNodeAfterClear()
    {
        _wasClearedAndDisabled = true;
        if (forceDebugLogs || enableDebugLogs)
            Debug.Log($"[NodeClickHandler] Disabled node after clear: '{name}'");
    }

    private static void DisableNodesWithGuid(string guid)
    {
        if (string.IsNullOrEmpty(guid)) return;
        var nodes = GameObject.FindObjectsOfType<NodeClickHandler>(includeInactive: true);
        int count = 0;
        foreach (var n in nodes)
        {
            if (n != null && n.mapNode != null && n.mapNode.UniqueId == guid)
            {
                n.DisableNodeAfterClear();
                count++;
            }
        }
        if (count > 0)
            Debug.Log($"[NodeClickHandler] Disabled {count} node(s) with GUID {guid}");
    }

    private void DisableNodeOnCombatEntry(string guid)
    {
        _wasClearedAndDisabled = true;
        if (forceDebugLogs || enableDebugLogs)
            Debug.Log($"[NodeClickHandler] Disabled node on combat entry: '{name}' guid={guid} scene={sceneToLoad}");
    }
    
    private void ShowNodeInfo()
    {
    if (cameraController != null)
        {
            cameraController.CenterOnPosition(transform.position);
        }
        
    if (nodeInfoUI != null)
        {
            nodeInfoUI.ShowInfo(GetNodeName(), GetNodeDescription(), transform.position);
        }
    }
    
    private string GetNodeName()
    {
        switch (nodeType)
        {
            case NodeType.Start: return "Start";
            case NodeType.Enemy: return "Griffon";
            case NodeType.Elite: return "Minotaur";
            case NodeType.Shop: return "Aeolus Wind Shop";
            case NodeType.Treasure: return "Treasure Chest";
            case NodeType.Heal: return "Asclepius Fountain";
            case NodeType.Gambling: return "Lottery";
            case NodeType.Challenge: return "Challenge";
            case NodeType.Boss: return "Athena";
            default: return "Unknown";
        }
    }
    
    private string GetNodeDescription()
    {
        switch (nodeType)
        {
            case NodeType.Start: return "Your starting point";
            case NodeType.Enemy: return "Half-eagle, half-lion. Proud and narcissistic, confident in both his strength and his beauty.";
            case NodeType.Elite: return "A serious figure with the head of a bull, struggling with a crisis of self-identity.";
            case NodeType.Shop: return "Here, amidst the whispering winds, are kept the gifts of the Lord of the Air himself. Come in and see what the wind has brought you!";
            case NodeType.Treasure: return "Someone left a chest full of drachma here.";

            case NodeType.Heal: return "Fountain of Asclepius. Taste its waters and gain new strength for your travels!";
            case NodeType.Gambling: return "In a world where the gods play with the fates of humans, even Dionysus smiles upon those who love risk and surprises.Open your bag because a gift from the gods may be waiting there!";
            case NodeType.Challenge: return "Special challenges";
            case NodeType.Boss: return "Goddess of Wisdom – always one step ahead. Watch your remaining turns carefully.";
            default: return "Unknown location";
        }
    }

    private bool IsSceneBlocked(string sceneName)
    {
        if(_healBlock && sceneName == "HealingDesign") return false;
        if(_shopBlock && sceneName == "ShopScene") return false;

        return true;
    }

    private void BlockScene(string scene)
    {
        if (scene == "Shop") _shopBlock = true;
        if (scene == "Heal") _healBlock = true;
    }
}
