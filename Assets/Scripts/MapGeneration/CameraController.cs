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
    public float xOffset = 0f;

    [Header("Zoom Settings")]
    public float minZoom = 5f;
    public float maxZoom = 20f;
    public float zoomSpeed = 10f;
    [Tooltip("Smoothing for zoom transitions (seconds), lower = snappier")] public float zoomSmoothTime = 0.12f;

    // Dragging and inertia removed per request

    [Header("Ground Plane")]
    [Tooltip("Y height of the ground plane used for mouse-to-world")] public float groundY = 0f;

    private Camera cam;
    private float targetOrthoSize;
    private float orthoVel; // ref for SmoothDamp
    private float targetFov;
    private float fovVel; // ref for SmoothDamp
    private Vector3 lastMousePosition;
    
    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            enabled = false;
        }
        else
        {
            if (cam.orthographic)
            {
                cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
                targetOrthoSize = cam.orthographicSize;
            }
            else
            {
                cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minZoom, maxZoom);
                targetFov = cam.fieldOfView;
            }
        }
        
        SetBoundsFromObject();
    }
    
    void Update()
    {
        HandleDragging();
        HandleZoom();
    }
    
    Vector3 GetMouseWorldPosition()
    {
        if (cam == null) return transform.position;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, new Vector3(0f, groundY, 0f));
        if (plane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }
        return transform.position;
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
    
    void HandleZoom()
    {
        if (cam == null) return;
        float scroll = Input.mouseScrollDelta.y;

        // Capture mouse world ref before changing zoom
        Vector3 mouseWorldRef = GetMouseWorldPosition();
        
        if (Mathf.Abs(scroll) > 1e-4f)
        {
            if (cam.orthographic)
            {
                targetOrthoSize = Mathf.Clamp(targetOrthoSize - scroll * zoomSpeed, minZoom, maxZoom);
            }
            else
            {
                targetFov = Mathf.Clamp(targetFov - scroll * (zoomSpeed * 2f), minZoom, maxZoom);
            }
        }

        if (cam.orthographic)
        {
            float prev = cam.orthographicSize;
            float next = Mathf.SmoothDamp(prev, targetOrthoSize, ref orthoVel, zoomSmoothTime);
            if (!Mathf.Approximately(prev, next))
            {
                cam.orthographicSize = next;
                // keep mouse pivot stable in ortho
                Vector3 toMouse = mouseWorldRef - transform.position;
                float factor = 1f - (prev / Mathf.Max(1e-4f, next));
                Vector3 pos = transform.position + new Vector3(toMouse.x, 0f, toMouse.z) * factor;
                transform.position = ClampToBounds(pos);
            }
        }
        else
        {
            float prev = cam.fieldOfView;
            float next = Mathf.SmoothDamp(prev, targetFov, ref fovVel, zoomSmoothTime);
            if (!Mathf.Approximately(prev, next))
            {
                cam.fieldOfView = next;
                // optional: we could approximate pivot preservation for perspective, but smoothing alone usually suffices
                transform.position = ClampToBounds(transform.position);
            }
        }
    }

    // Inertia removed
    
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
