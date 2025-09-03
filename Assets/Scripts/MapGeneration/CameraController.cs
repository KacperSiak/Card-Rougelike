using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Movement Limits")]
    public Vector2 minBounds = new Vector2(-50f, -50f);
    public Vector2 maxBounds = new Vector2(50f, 50f);
    public bool autoSetBounds = false;
    
    [Header("Bounds from Object")]
    public GameObject boundsObject;
    
    [Header("Camera Height")]
    public float cameraHeight = 15f;
    
    [Header("Centering Settings")]
    public float xOffset = 0f;    [Header("Zoom Settings")]
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
        
        SetBoundsFromObject();
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
            newPosition.x += deltaWorld.x;
            newPosition.z += deltaWorld.z;
            
            newPosition = ClampToBounds(newPosition);
            
            transform.position = newPosition;
        }
    }
    
    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = transform.position.y;
        return cam.ScreenToWorldPoint(mousePos);
    }
    
    Vector3 ClampToBounds(Vector3 position)
    {
        if (boundsObject != null)
        {
            Collider collider = boundsObject.GetComponent<Collider>();
            if (collider != null)
            {
                Bounds bounds = collider.bounds;
                
                position.x = Mathf.Clamp(position.x, 
                    bounds.min.x, 
                    bounds.max.x);
                position.z = Mathf.Clamp(position.z, 
                    bounds.min.z, 
                    bounds.max.z);
                
                return position;
            }
        }
        
        position.x = Mathf.Clamp(position.x, minBounds.x, maxBounds.x);
        position.z = Mathf.Clamp(position.z, minBounds.y, maxBounds.y);
        
        return position;
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
            
            transform.position = ClampToBounds(transform.position);
        }
    }
    
    public void SetBoundsFromObject()
    {
        if (boundsObject == null) return;
        
        Renderer renderer = boundsObject.GetComponent<Renderer>();
        Collider collider = boundsObject.GetComponent<Collider>();
        
        Bounds bounds;
        
        if (renderer != null)
        {
            bounds = renderer.bounds;
        }
        else if (collider != null)
        {
            bounds = collider.bounds;
        }
        else
        {
            Debug.LogWarning("[CameraController] Bounds object nie ma Renderer ani Collider!");
            return;
        }
        
        minBounds = new Vector2(bounds.min.x, bounds.min.z);
        maxBounds = new Vector2(bounds.max.x, bounds.max.z);
        
        Debug.Log($"[CameraController] Bounds ustawione na podstawie {boundsObject.name}: Min{minBounds}, Max{maxBounds}");
    }
    
    public void SetBoundsFromMap(MapGenerator mapGenerator)
    {
        if (!autoSetBounds) return;
        
        if (mapGenerator != null && mapGenerator.mapLevels != null && mapGenerator.mapLevels.Count > 0)
        {
            float minX = float.MaxValue, maxX = float.MinValue;
            float minZ = float.MaxValue, maxZ = float.MinValue;
            
            foreach (var level in mapGenerator.mapLevels)
            {
                foreach (var node in level)
                {
                    if (node.Position.x < minX) minX = node.Position.x;
                    if (node.Position.x > maxX) maxX = node.Position.x;
                    if (node.Position.y < minZ) minZ = node.Position.y;
                    if (node.Position.y > maxZ) maxZ = node.Position.y;
                }
            }
            
            minBounds = new Vector2(minX - 10f, minZ - 10f);
            maxBounds = new Vector2(maxX + 10f, maxZ + 10f);
        }
    }
    
    public void CenterOnPosition(Vector3 worldPosition)
    {
        Vector3 targetPosition = transform.position;
        targetPosition.x = worldPosition.x + xOffset;
        targetPosition.z = worldPosition.z;
        
        targetPosition = ClampToBounds(targetPosition);
        
        StartCoroutine(SmoothMoveToPosition(targetPosition));
    }
    
    private System.Collections.IEnumerator SmoothMoveToPosition(Vector3 targetPos)
    {
        Vector3 startPos = transform.position;
        float duration = 0.5f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = Mathf.SmoothStep(0f, 1f, t);
            
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
        
        transform.position = targetPos;
    }
}
