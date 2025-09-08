using UnityEngine;

public class HoverHighlighter : MonoBehaviour
{
    [Header("Highlight Effect")]
    public Color baseColor = Color.white;
    public Color highlightColor = Color.yellow;
    
    [Header("Icon Animation")]
    public Transform iconToAnimate;
    public float liftHeight = 0.5f;
    public float animationSpeed = 3f;
    
    [Header("Settings")]
    public LayerMask raycastLayer = -1;
    [Tooltip("Gdy true, wyłącza efekt hover, jeśli gracz stoi na tym polu")]
    public bool disableWhenOccupied = true;
    
    private Renderer objectRenderer;
    private Collider objectCollider;
    private bool isHighlighted = false;
    private MaterialPropertyBlock propertyBlock;
    private static readonly int BaseColorProperty = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorProperty = Shader.PropertyToID("_Color");
    
    private Vector3 originalIconPosition;
    private Vector3 targetIconPosition;
    private bool isAnimating = false;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        objectCollider = GetComponent<Collider>(); // do not look for references in runtime
        
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
        
        if (iconToAnimate != null)
        {
            originalIconPosition = iconToAnimate.localPosition;
            targetIconPosition = originalIconPosition;
        }
        
        SetColor(false);
    }

    void Update()
    {
        if (Camera.main == null)
        {
            return;
        }

        if (disableWhenOccupied && NodeClickHandler.currentPlayerNode != null && NodeClickHandler.currentPlayerNode == this.transform)
        {
            if (isHighlighted)
            {
                SetHighlight(false);
            }
            AnimateIcon();
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
        
        AnimateIcon();
    }
    
    private void SetHighlight(bool highlight)
    {
        if (objectRenderer == null) return;
        
        if (highlight == isHighlighted) return;
        
        SetColor(highlight);
        SetIconTarget(highlight);
        
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
    
    public void ForceHighlight(bool highlight)
    {
        SetHighlight(highlight);
    }
    
    private void SetIconTarget(bool highlight)
    {
        if (iconToAnimate == null) return;
        
        if (highlight)
        {
            targetIconPosition = originalIconPosition + Vector3.up * liftHeight;
        }
        else
        {
            targetIconPosition = originalIconPosition;
        }
        
        isAnimating = true;
    }
    
    private void AnimateIcon()
    {
        if (iconToAnimate == null || !isAnimating) return;
        
        iconToAnimate.localPosition = Vector3.Lerp(
            iconToAnimate.localPosition, 
            targetIconPosition, 
            animationSpeed * Time.deltaTime
        );
        
        if (Vector3.Distance(iconToAnimate.localPosition, targetIconPosition) < 0.01f)
        {
            iconToAnimate.localPosition = targetIconPosition;
            isAnimating = false;
        }
    }
}
