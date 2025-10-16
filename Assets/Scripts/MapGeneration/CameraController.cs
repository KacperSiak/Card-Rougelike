using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Movement Limits")]
    public bool autoSetBounds = false;
    
    [Header("Bounds from Object")]
    public GameObject boundsObject;
    
    [Header("Camera Height")]
    public float cameraHeight = 15f;
    
    [Header("Centering Settings")]
    public float xOffset = 0f;
    [Tooltip("Duration (seconds) for smooth centering movement")]
    public float centerMoveDuration = 0.5f;

    [Header("Zoom Settings")]
    public float minZoom = 5f;
    public float maxZoom = 20f;
    public float zoomSpeed = 10f;
    [Tooltip("Smoothing for zoom transitions (seconds), lower = snappier")] public float zoomSmoothTime = 0.12f;


    [Header("Ground Plane")]
    [Tooltip("Y height of the ground plane used for mouse-to-world")] public float groundY = 0f;

    private Camera cam;
    private float targetOrthoSize;
    private float orthoVel;
    private float targetFov;
    private float fovVel;
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
                Vector3 mouseWorldAfter = GetMouseWorldPosition();
                Vector3 pivotDelta = mouseWorldRef - mouseWorldAfter;
                Vector3 pos = transform.position + new Vector3(pivotDelta.x, 0f, pivotDelta.z);
                transform.position = ClampToBounds(pos);
            }
        }
    }
    
    public void SetBoundsFromObject()
    {
        if (boundsObject == null) return;
        
        Renderer renderer = boundsObject.GetComponent<Renderer>();
        Collider collider = boundsObject.GetComponent<Collider>();
        
        if (renderer == null && collider == null)
        {
            Debug.LogWarning("[CameraController] Bounds object has neither Renderer nor Collider!");
        }
    }
    
    public void SetBoundsFromMap(MapGenerator mapGenerator)
    {
        if (!autoSetBounds) return;
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
        float duration = centerMoveDuration;
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
