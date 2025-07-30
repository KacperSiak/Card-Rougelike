using UnityEngine;

/// <summary>
/// Skrypt diagnostyczny do sprawdzania problemów z HoverHighlighter
/// </summary>
public class HoverHighlighterDebugger : MonoBehaviour
{
    [Header("Debug Settings")]
    public bool showDebugRays = false;
    public bool logRaycastHits = false;
    public LayerMask debugRaycastLayer = -1;
    
    private Camera debugCamera;
    private GameObject lastHoveredObject = null; // Ostatnio najechany obiekt
    
    void Start()
    {
        debugCamera = Camera.main;
        if (debugCamera == null)
        {
            debugCamera = FindFirstObjectByType<Camera>();
        }
    }
    
    void Update()
    {
        if (debugCamera == null) return;
        
        Ray ray = debugCamera.ScreenPointToRay(Input.mousePosition);
        
        // Sprawdź czy trafiliśmy w obiekt z HoverHighlighter
        GameObject currentHoveredObject = null;
        bool hitHighlightableObject = false;
        
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, debugRaycastLayer))
        {
            var highlighter = hit.transform.GetComponent<HoverHighlighter>();
            if (highlighter != null)
            {
                hitHighlightableObject = true;
                currentHoveredObject = hit.transform.gameObject;
            }
        }
        
        // Pokaż promień TYLKO gdy najeżdżasz na obiekt z HoverHighlighter
        if (showDebugRays && hitHighlightableObject)
        {
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green, 0.1f);
        }
        
        // Loguj TYLKO gdy zmienia się najechany obiekt
        if (logRaycastHits && currentHoveredObject != lastHoveredObject)
        {
            if (currentHoveredObject != null)
            {
                Debug.Log($"HOVER: Najechano na {currentHoveredObject.name}");
                
                // Szczegółowe info tylko gdy zmienia się obiekt
                RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, debugRaycastLayer);
                if (hits.Length > 0)
                {
                    Debug.Log($"Raycast trafił w {hits.Length} obiektów:");
                    for (int i = 0; i < hits.Length; i++)
                    {
                        var hitInfo = hits[i];
                        var highlighter = hitInfo.transform.GetComponent<HoverHighlighter>();
                        var layer = LayerMask.LayerToName(hitInfo.transform.gameObject.layer);
                        
                        Debug.Log($"  {i}: {hitInfo.transform.name} (layer: {hitInfo.transform.gameObject.layer}-{layer}, " +
                                 $"distance: {hitInfo.distance:F2}, " +
                                 $"hasHighlighter: {highlighter != null}, " +
                                 $"highlighterEnabled: {(highlighter != null ? highlighter.enabled : false)})");
                    }
                }
            }
            else if (lastHoveredObject != null)
            {
                Debug.Log("HOVER: Opuszczono obiekt");
            }
            
            lastHoveredObject = currentHoveredObject;
        }
        
        // Sprawdź obiekt bezpośrednio pod myszą
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out RaycastHit clickHit, Mathf.Infinity, debugRaycastLayer))
            {
                Debug.Log($"Kliknięto w obiekt: {clickHit.transform.name} na pozycji {clickHit.point}");
                
                var highlighter = clickHit.transform.GetComponent<HoverHighlighter>();
                if (highlighter != null)
                {
                    Debug.Log($"Obiekt ma HoverHighlighter: enabled={highlighter.enabled}");
                }
                else
                {
                    Debug.Log("Obiekt NIE ma komponentu HoverHighlighter");
                }
            }
            else
            {
                Debug.Log("Raycast nie trafił w żaden obiekt");
            }
        }
    }
    
    void OnGUI()
    {
        if (debugCamera == null) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 300, 150));
        GUILayout.Label("HoverHighlighter Debugger");
        
        showDebugRays = GUILayout.Toggle(showDebugRays, "Show Debug Rays");
        logRaycastHits = GUILayout.Toggle(logRaycastHits, "Log Raycast Hits");
        
        GUILayout.Label($"Camera: {debugCamera.name}");
        GUILayout.Label($"Mouse Position: {Input.mousePosition}");
        
        Ray ray = debugCamera.ScreenPointToRay(Input.mousePosition);
        GUILayout.Label($"Ray Origin: {ray.origin}");
        GUILayout.Label($"Ray Direction: {ray.direction}");
        
        GUILayout.EndArea();
    }
}
