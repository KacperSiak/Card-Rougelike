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
        if (panel == null) return;
        
        if (titleText != null)
            titleText.text = title;
            
        if (descriptionText != null)
            descriptionText.text = description;
        
        panel.SetActive(true);
        hideTimer = hideAfterSeconds;
    }
    
    public void HideInfo()
    {
        if (panel != null)
            panel.SetActive(false);
    }
}
