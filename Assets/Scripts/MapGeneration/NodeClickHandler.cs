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
    
    private static bool markerInitialized = false;
    private static MapGenerator cachedMapGenerator;

    void Start()
    {
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }
        
        // Marker jest teraz tworzony przez MapGenerator
        // Nie trzeba już inicjalizować tutaj
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
        markerInitialized = true;
    }
    
    void CreateDefaultMarker()
    {
        playerMarker = GameObject.CreatePrimitive(PrimitiveType.Cube);
        playerMarker.name = "Marker";
        playerMarker.transform.localScale = Vector3.one * 0.5f;
        
        Renderer renderer = playerMarker.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.red;
        }
        
        Destroy(playerMarker.GetComponent<Collider>());
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
            else
            {
                Debug.Log("Cannot move to this node - not connected");
            }
        }
    }

    bool CanMoveToThisNode()
    {
        if (currentPlayerNode == null) 
        {
            Debug.Log("First move - can go anywhere");
            return true;
        }
        
        if (cachedMapGenerator == null)
        {
            cachedMapGenerator = FindObjectOfType<MapGenerator>();
        }
        
        if (cachedMapGenerator == null || cachedMapGenerator.mapLevels == null) 
        {
            Debug.LogWarning("MapGenerator not found");
            return true;
        }
        
        Vector2 currentPos = new Vector2(currentPlayerNode.position.x, currentPlayerNode.position.z);
        Vector2 targetPos = new Vector2(transform.position.x, transform.position.z);
        
        Debug.Log($"Checking move from {currentPos} to {targetPos}");
        
        MapNode currentMapNode = null;
        
        foreach (var level in cachedMapGenerator.mapLevels)
        {
            foreach (var node in level)
            {
                Vector2 nodePos = new Vector2(node.Position.x, node.Position.y);
                float distanceToCurrent = Vector2.Distance(nodePos, currentPos);
                
                if (distanceToCurrent < 0.5f)
                {
                    currentMapNode = node;
                    Debug.Log($"Found current node at {nodePos}, connections: {node.ConnectedNodes.Count}");
                    break;
                }
            }
            if (currentMapNode != null) break;
        }
        
        if (currentMapNode == null)
        {
            Debug.LogWarning("Current player node not found in map data");
            return false;
        }
        
        foreach (var connectedNode in currentMapNode.ConnectedNodes)
        {
            Vector2 connectedPos = new Vector2(connectedNode.Position.x, connectedNode.Position.y);
            float distanceToTarget = Vector2.Distance(connectedPos, targetPos);
            
            Debug.Log($"Checking connection to {connectedPos}, distance to target: {distanceToTarget}");
            
            if (distanceToTarget < 0.5f)
            {
                Debug.Log("Valid connection found!");
                return true;
            }
        }
        
        Debug.Log("No valid connection found");
        return false;
    }
    
    // Resetuj cache gdy mapa się regeneruje
    public static void ResetCache()
    {
        cachedMapGenerator = null;
        markerInitialized = false;
        currentPlayerNode = null;
    }

    void MovePlayerHere()
    {
        currentPlayerNode = transform;
        
        if (playerMarker == null)
        {
            InitializePlayerMarker();
        }
        
        SavePlayerPosition();
        
        if (playerMarker != null)
        {
            playerMarker.transform.position = transform.position;
            playerMarker.SetActive(true);
        }
        
        Debug.Log($"Player moved to: {transform.position}");
    }
    
    void SavePlayerPosition()
    {
        Vector3 pos = transform.position;
        PlayerPrefs.SetFloat("PlayerPosX", pos.x);
        PlayerPrefs.SetFloat("PlayerPosY", pos.y);
        PlayerPrefs.SetFloat("PlayerPosZ", pos.z);
        PlayerPrefs.Save();
        Debug.Log($"Player position saved: {pos}");
    }
    
    public static void LoadPlayerPosition()
    {
        if (PlayerPrefs.HasKey("PlayerPosX"))
        {
            float x = PlayerPrefs.GetFloat("PlayerPosX");
            float y = PlayerPrefs.GetFloat("PlayerPosY");  
            float z = PlayerPrefs.GetFloat("PlayerPosZ");
            Vector3 savedPos = new Vector3(x, y, z);
            
            // Znajdź node najbliżej zapisanej pozycji
            FindNodeAtPosition(savedPos);
            Debug.Log($"Player position loaded: {savedPos}");
        }
    }
    
    static void FindNodeAtPosition(Vector3 savedPos)
    {
        MapGenerator mapGen = FindObjectOfType<MapGenerator>();
        if (mapGen == null) return;
        
        float closestDistance = float.MaxValue;
        Transform closestNode = null;
        
        // Sprawdź wszystkie nody w scenie
        NodeClickHandler[] allNodes = FindObjectsOfType<NodeClickHandler>();
        foreach (var nodeHandler in allNodes)
        {
            float distance = Vector3.Distance(nodeHandler.transform.position, savedPos);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNode = nodeHandler.transform;
            }
        }
        
        if (closestNode != null && closestDistance < 1f)
        {
            currentPlayerNode = closestNode;
            
            if (playerMarker == null)
            {
                NodeClickHandler nodeHandler = closestNode.GetComponent<NodeClickHandler>();
                if (nodeHandler != null)
                {
                    nodeHandler.InitializePlayerMarker();
                }
            }
            
            if (playerMarker != null)
            {
                playerMarker.transform.position = closestNode.position;
                playerMarker.SetActive(true);
            }
        }
    }

    void LoadScene()
    {
        Debug.Log($"Clicked on node - loading scene: {sceneToLoad}");
        
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("No scene name");
        }
    }
}
