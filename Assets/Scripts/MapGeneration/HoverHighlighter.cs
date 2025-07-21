using UnityEngine;

public class HoverHighlighter : MonoBehaviour
{
    [Header("Highlight Effect")]
    public Color baseColor = Color.white;
    public Color highlightColor = Color.yellow;
    
    [Header("Settings")]
    public LayerMask raycastLayer = -1;
    
    private Renderer objectRenderer;
    private Collider objectCollider;
    private bool isHighlighted = false;
    private MaterialPropertyBlock propertyBlock;
    private static readonly int BaseColorProperty = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorProperty = Shader.PropertyToID("_Color");

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        objectCollider = GetComponent<Collider>();
        
        // Sprawdź komponenty
        if (objectRenderer == null)
        {
            Debug.LogError($"HoverHighlighter: Missing Renderer component on object {gameObject.name}");
            enabled = false;
            return;
        }
        
        if (objectCollider == null)
        {
            Debug.LogError($"HoverHighlighter: Missing Collider component on object {gameObject.name}");
            enabled = false;
            return;
        }
        
        propertyBlock = new MaterialPropertyBlock();
        
        SetColor(false);
    }

    void Update()
    {
        if (Camera.main == null)
        {
            Debug.LogError("HoverHighlighter: Missing main camera");
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, raycastLayer))
        {
            if (hit.transform == transform)
            {
                if (!isHighlighted)
                {
                    SetHighlight(true);
                }
            }
            else
            {
                if (isHighlighted)
                {
                    SetHighlight(false);
                }
            }
        }
        else
        {
            if (isHighlighted)
            {
                SetHighlight(false);
            }
        }
    }
    
    private void SetHighlight(bool highlight)
    {
        if (objectRenderer == null) return;
        
        if (highlight == isHighlighted) return; // Nie zmieniaj jeśli stan jest już właściwy
        
        SetColor(highlight);
        
        isHighlighted = highlight;
        Debug.Log($"{(highlight ? "Highlighted" : "Removed highlight from")} object: {gameObject.name}");
    }
    
    private void SetColor(bool highlight)
    {
        if (propertyBlock == null) return;
        
        // Wybierz kolor w zależności od stanu
        Color targetColor = highlight ? highlightColor : baseColor;
        
        objectRenderer.GetPropertyBlock(propertyBlock);
        
        // Ustaw kolor podstawowy (wspiera URP i Built-in)
        propertyBlock.SetColor(BaseColorProperty, targetColor);
        propertyBlock.SetColor(ColorProperty, targetColor);
        
        objectRenderer.SetPropertyBlock(propertyBlock);
    }
    
    private void OnDisable()
    {
        if (isHighlighted && objectRenderer != null)
        {
            SetHighlight(false);
        }
    }
    
    // Metoda do ręcznego przełączania podświetlenia (dla debugowania)
    [ContextMenu("Toggle Highlight")]
    public void ToggleHighlight()
    {
        SetHighlight(!isHighlighted);
    }
    
    // Metoda do wymuszenia podświetlenia (dla innych skryptów)
    public void ForceHighlight(bool highlight)
    {
        SetHighlight(highlight);
    }
}
