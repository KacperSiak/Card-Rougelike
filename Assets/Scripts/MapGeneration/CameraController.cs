using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Movement Limits")]
    public Vector2 minBounds = new Vector2(-50f, -50f);
    public Vector2 maxBounds = new Vector2(50f, 10f); 
    
    [Header("Camera Height")]
    public float cameraHeight = 15f;
    
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
            newPosition.x += deltaWorld.x;
            newPosition.z += deltaWorld.z;
            
            newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
            newPosition.z = Mathf.Clamp(newPosition.z, minBounds.y, maxBounds.y);
            
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
            
            float padding = 10f;
            minBounds = new Vector2(minX - padding, minZ - padding);
            maxBounds = new Vector2(maxX + padding, maxZ + padding);
        }
    }
    
    public void CenterOnPosition(Vector3 worldPosition)
    {
        Vector3 targetPosition = transform.position;
        targetPosition.z = worldPosition.z;
        
        targetPosition.z = Mathf.Clamp(targetPosition.z, minBounds.y, maxBounds.y);
        
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
