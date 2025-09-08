using UnityEngine;
using UnityEngine.SceneManagement;
using MapSystem;
using Systems.SceneManagement;

public class NodeClickHandler : MonoBehaviour
{
    [Header("Scene To Load")]
    public string sceneToLoad = "";
    
    [Header("Player Tracking")]
    public static Transform currentPlayerNode;
    public static GameObject playerMarker;
    public GameObject playerMarkerPrefab;
    
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
    
    private static bool isPlayerMoving = false;
    

    void Start()
    {
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }
    }
    
    void InitializePlayerMarker()
    {
        if (playerMarker == null)
        {
            if (playerMarkerPrefab != null)
            {
                playerMarker = Instantiate(playerMarkerPrefab);
                playerMarker.name = "PlayerMarker";
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
            if (CanMoveToThisNode())
            {
                MovePlayerHere();
            }
        }
    }

    bool CanMoveToThisNode()
    {
        if (isPlayerMoving)
        {
            return false;
        }
        
        if (currentPlayerNode == null) 
        {
            return true;
        }
        
        if (mapGenerator == null || mapGenerator.mapLevels == null) 
        {
            return false;
        }
        
        if (mapManager != null && !mapManager.HasTimeLeft())
        {
            Debug.Log("[NodeClickHandler] No more time left");
            return false;
        }
        
        Vector2 currentPos = new Vector2(currentPlayerNode.position.x, currentPlayerNode.position.z);
        Vector2 targetPos = new Vector2(transform.position.x, transform.position.z);
        
        MapNode currentMapNode = null;
        
        foreach (var level in mapGenerator.mapLevels)
        {
            foreach (var node in level)
            {
                Vector2 nodePos = new Vector2(node.Position.x, node.Position.y);
                float distanceToCurrent = Vector2.Distance(nodePos, currentPos);
                
                if (distanceToCurrent < 0.5f)
                {
                    currentMapNode = node;
                    break;
                }
            }
            if (currentMapNode != null) break;
        }
        
        if (currentMapNode == null)
        {
            return false;
        }
        
        foreach (var connectedNode in currentMapNode.ConnectedNodes)
        {
            Vector2 connectedPos = new Vector2(connectedNode.Position.x, connectedNode.Position.y);
            float distanceToTarget = Vector2.Distance(connectedPos, targetPos);
            
            if (distanceToTarget < 0.5f)
            {
                return true;
            }
        }
        
        return false;
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
        // Save player position on every completed move
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

        // Save player position on every completed move
        if (mapSaveManager != null)
        {
            mapSaveManager.SaveCurrentMap();
        }

        LoadScene();
    }

    void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            if (IsCombatNode(nodeType) || LooksLikeCombatScene(sceneToLoad))
            {
                CombatManager.ApplyNodeType(nodeType);
            }
            
            if (mapSaveManager != null)
            {
                mapSaveManager.SaveCurrentMap();
            }
            
            if (SceneLoader.Instance != null)
            {
                Debug.Log("Using additive scene activation for: " + sceneToLoad);
                SceneLoader.Instance.manager.ActivateScene(sceneToLoad);
                if (IsCombatNode(nodeType) || LooksLikeCombatScene(sceneToLoad))
                {
                    CombatManager.ApplyNodeType(nodeType);
                }
                return;
            }
            
            Debug.Log("SceneLoader not found, using normal scene loading for: " + sceneToLoad);
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    static bool IsCombatNode(NodeType t)
    {
        return t == NodeType.Enemy || t.ToString() == "Elite" || t.ToString() == "Boss";
    }

    static bool LooksLikeCombatScene(string scene)
    {
        return !string.IsNullOrEmpty(scene) && (scene.IndexOf("combat", System.StringComparison.OrdinalIgnoreCase) >= 0 || scene.IndexOf("battle", System.StringComparison.OrdinalIgnoreCase) >= 0);
    }
    
    public void TriggerLoadScene()
    {
        LoadScene();
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
            case NodeType.Enemy: return "Enemy";
            case NodeType.Elite: return "Elite Enemy";
            case NodeType.Shop: return "Shop";
            case NodeType.Treasure: return "Treasure";
            case NodeType.Heal: return "Rest Site";
            case NodeType.Gambling: return "Gambling";
            case NodeType.Challenge: return "Challenge";
            case NodeType.Boss: return "Boss";
            default: return "Unknown";
        }
    }
    
    private string GetNodeDescription()
    {
        switch (nodeType)
        {
            case NodeType.Start: return "Your starting point";
            case NodeType.Enemy: return "Fight enemies";
            case NodeType.Elite: return "Fight elite enemies for better rewards";
            case NodeType.Shop: return "Buy items and cards";
            case NodeType.Treasure: return "Find treasure";
            case NodeType.Heal: return "Rest and heal";
            case NodeType.Gambling: return "Take risks for rewards";
            case NodeType.Challenge: return "Special challenges";
            case NodeType.Boss: return "Final boss";
            default: return "Unknown location";
        }
    }
}
