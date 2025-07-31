using UnityEngine;
using UnityEngine.SceneManagement;
using MapSystem;

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

    [Header("Node Info")]
    public NodeType nodeType = NodeType.Enemy;
    

    void Start()
    {
        // tymczasowy fallback
        if (mapGenerator == null)
        {
            mapGenerator = FindFirstObjectByType<MapGenerator>();
        }
        
        if (mapManager == null)
        {
            mapManager = FindFirstObjectByType<MapManager>();
        }
        
        
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

    private Vector3 mouseDownPosition;
    private float dragThreshold = 10f;

    void OnMouseDown()
    {
        mouseDownPosition = Input.mousePosition;
    }
    
    void OnMouseUp()
    {
        float dragDistance = Vector3.Distance(Input.mousePosition, mouseDownPosition);
        
        if (dragDistance < dragThreshold)
        {
            if (CanMoveToThisNode())
            {
                MovePlayerHere();
                LoadScene();
            }
        }
    }

    bool CanMoveToThisNode()
    {
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
            Vector3 newPos = toPos3D;
            newPos.y = 0.2f;
            playerMarker.transform.position = newPos;
            
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
    }

    void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            var mapSaveManager = FindFirstObjectByType<MapSaveManager>();
            if (mapSaveManager != null)
            {
                mapSaveManager.SaveBeforeSceneChange();
                Debug.Log("[NodeClickHandler] Map saved by MapSaveManager");
            }
            
            SceneManager.LoadScene(sceneToLoad);
        }
    }
    
    public void TriggerLoadScene()
    {
        LoadScene();
    }
}
