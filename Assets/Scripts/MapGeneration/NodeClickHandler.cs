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
    // Logical backing MapNode of the player's current position (used if the Transform has no NodeClickHandler, e.g. Start node prefab without handler)
    public static MapNode currentPlayerMapNode;
    
    [Header("References")]
    public MapGenerator mapGenerator;
    public MapManager mapManager;
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
    
    private static bool isPlayerMoving = false;
    private bool _shopBlock = false;
    private bool _healBlock = false;

    // Track cleared combat nodes so revisiting doesn't reload scenes
    private static readonly HashSet<NodeClickHandler> _clearedCombatNodes = new HashSet<NodeClickHandler>();
    // Remember which combat node we entered last to mark it cleared on win
    public static NodeClickHandler lastEnteredCombatNode;
    
    [Header("Debug")]
    [Tooltip("Enable verbose debug logs for movement / adjacency checks.")]
    public bool enableDebugLogs = false;
    
    // Global emergency debug switch (user requested always-on logs)
    public static bool forceDebugLogs = true;
    

    void Start()
    {
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }

        ConsumablesEventSystem.OnSceneBlock.AddListener(BlockScene);

        // Warn early if the generator failed to assign the backing MapNode reference.
        if (mapNode == null)
        {
            Debug.LogWarning($"[NodeClickHandler] mapNode reference is NULL on '{name}'. Movement adjacency will fail for this node.");
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

    bool CanMoveToThisNode()
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
        
    // Fast path: use MapNode references instead of scanning the whole map
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

    // Adjacent if the target's MapNode is in the current node's connection list
    bool reachable = sourceMapNode.ConnectedNodes.Contains(mapNode);
    if (enableDebugLogs)
    {
        int currentConn = sourceMapNode.ConnectedNodes != null ? sourceMapNode.ConnectedNodes.Count : -1;
        Debug.Log($"[NodeClickHandler] Adjacency check: from '{currentPlayerNode.name}' -> '{name}' ({nodeType}) | currentConnections={currentConn} | reachable={reachable}");
        if (!reachable)
        {
            // Optional deeper dump once per failed attempt
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
    currentPlayerMapNode = mapNode; // keep logical map node in sync
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

    void LoadScene()
    {
        // If this is a combat node that was already cleared by a previous win,
        // allow entering the tile but do not load any scene (stay on map)
        if (IsCombatNode(nodeType) && _clearedCombatNodes.Contains(this))
        {
            Debug.Log($"[NodeClickHandler] Node '{name}' already cleared. Staying on map.");
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

    // Called by RoundManager when player wins a combat
    public static void MarkLastCombatNodeClearedIfAny()
    {
        if (lastEnteredCombatNode != null)
        {
            _clearedCombatNodes.Add(lastEnteredCombatNode);
            Debug.Log($"[NodeClickHandler] Marked node '{lastEnteredCombatNode.name}' as cleared");
            lastEnteredCombatNode = null;
        }
    }

    // Called when regenerating a new map (e.g., on defeat)
    public static void ClearClearedNodes()
    {
        _clearedCombatNodes.Clear();
        lastEnteredCombatNode = null;
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
