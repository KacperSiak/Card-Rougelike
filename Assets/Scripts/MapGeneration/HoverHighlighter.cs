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
        
        if (objectRenderer == null)
        {
            enabled = false;
            return;
        }
        
        if (objectCollider == null)
        {
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
        
        if (highlight == isHighlighted) return;
        
        SetColor(highlight);
        
        isHighlighted = highlight;
    }
    
    private void SetColor(bool highlight)
    {
        if (propertyBlock == null) return;
        
        Color targetColor = highlight ? highlightColor : baseColor;
        
        objectRenderer.GetPropertyBlock(propertyBlock);
        
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
    
    [ContextMenu("Toggle Highlight")]
    public void ToggleHighlight()
    {
        SetHighlight(!isHighlighted);
    }
    
    public void ForceHighlight(bool highlight)
    {
        SetHighlight(highlight);
    }
}
