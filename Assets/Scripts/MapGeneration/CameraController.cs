using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Movement Limits")]
    public Vector2 minBounds = new Vector2(0f, -50f);
    public Vector2 maxBounds = new Vector2(0f, 10f); 
    
    [Header("Zoom Settings")]
    public float minZoom = 5f;
    public float maxZoom = 20f;

    private Vector3 lastMousePosition;
    private Camera cam;
    
    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            enabled = false;
        }
    }
    
    void Update()
    {
        HandleDragging();
        HandleZoom();
    }
    
    void HandleDragging()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = GetMouseWorldPosition();
            lastMousePosition = mouseWorldPos;
        }
        
        if (Input.GetMouseButton(0))
        {
            Vector3 currentMouseWorld = GetMouseWorldPosition();
            Vector3 deltaWorld = lastMousePosition - currentMouseWorld;
            
            Vector3 newPosition = transform.position;
            newPosition.z += deltaWorld.z;
            
            newPosition.z = Mathf.Clamp(newPosition.z, minBounds.y, maxBounds.y); // limiting to bounds
            
            transform.position = newPosition;
        }
    }
    
    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = transform.position.y;
        return cam.ScreenToWorldPoint(mousePos);
    }
    
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        
        if (scroll != 0)
        {
            if (cam.orthographic)
            {
                cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - scroll * 2f, minZoom, maxZoom);
            }
            else
            {
                cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - scroll * 20f, minZoom, maxZoom);
            }
        }
    }
    
    public void SetBoundsFromMap(MapGenerator mapGenerator)
    {
        if (mapGenerator != null && mapGenerator.mapLevels != null && mapGenerator.mapLevels.Count > 0)
        {
            float minZ = float.MaxValue, maxZ = float.MinValue;
            
            foreach (var level in mapGenerator.mapLevels)
            {
                foreach (var node in level)
                {
                    if (node.Position.y < minZ) minZ = node.Position.y;
                    if (node.Position.y > maxZ) maxZ = node.Position.y;
                }
            }
            
            float padding = 10f;
            minBounds = new Vector2(0, minZ - padding);
            maxBounds = new Vector2(0, maxZ + padding);
        }
    }
}
