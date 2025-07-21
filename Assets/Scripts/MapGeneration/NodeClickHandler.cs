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

    void Start()
    {
        // Tymczasowy fallback
        if (mapGenerator == null)
        {
            mapGenerator = FindObjectOfType<MapGenerator>();
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
        currentPlayerNode = transform;
        
        if (playerMarker == null)
        {
            InitializePlayerMarker();
        }
        
        if (playerMarker != null)
        {
            playerMarker.transform.position = transform.position;
            playerMarker.SetActive(true);
        }
    }

    void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
