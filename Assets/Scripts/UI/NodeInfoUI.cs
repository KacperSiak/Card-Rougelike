using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NodeInfoUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject panel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Button closeButton;
    
    [Header("Settings")]
    public float hideAfterSeconds = 5f;
    
    private float hideTimer;
    
    void Start()
    {
        if (panel != null)
            panel.SetActive(false);
            
        if (closeButton != null)
            closeButton.onClick.AddListener(HideInfo);
    }
    
    void Update()
    {
        if (panel != null && panel.activeInHierarchy)
        {
            hideTimer -= Time.deltaTime;
            if (hideTimer <= 0 || Input.GetKeyDown(KeyCode.Escape))
            {
                HideInfo();
            }
        }
    }
    
    public void ShowInfo(string title, string description, Vector3 worldPos)
    {
        Debug.Log($"[NodeInfoUI] ShowInfo called - Title: {title}, Description: {description}");
        
        if (panel == null)
        {
            Debug.LogError("[NodeInfoUI] Panel is NULL!");
            return;
        }
        
        if (titleText != null)
        {
            titleText.text = title;
            Debug.Log($"[NodeInfoUI] Title text set to: {title}");
        }
        else
        {
            Debug.LogWarning("[NodeInfoUI] Title text is NULL!");
        }
            
        if (descriptionText != null)
        {
            descriptionText.text = description;
            Debug.Log($"[NodeInfoUI] Description text set to: {description}");
        }
        else
        {
            Debug.LogWarning("[NodeInfoUI] Description text is NULL!");
        }
        
        panel.SetActive(true);
        hideTimer = hideAfterSeconds;
        
        Debug.Log($"[NodeInfoUI] Panel activated! Hide timer set to {hideAfterSeconds} seconds");
    }
    
    public void HideInfo()
    {
        if (panel != null)
            panel.SetActive(false);
    }
}
