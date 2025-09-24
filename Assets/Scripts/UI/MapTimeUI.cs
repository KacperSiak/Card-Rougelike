using UnityEngine;
using TMPro;

public class MapTimeUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Text component do wyświetlania czasu")]
    public TextMeshProUGUI timeText;
    
    [Header("References")]
    public MapManager mapManager;
    
    [Header("Display Settings")]
    [Tooltip("Format wyświetlania czasu")]
    public string timeFormat = "Time: {0:F1} / {1:F1}";
    [Tooltip("Kolor tekstu gdy czas się kończy (czerwony)")]
    public Color lowTimeColor = Color.red;
    [Tooltip("Normalny kolor tekstu")]
    public Color normalColor = Color.white;
    [Tooltip("Próg poniżej którego tekst staje się czerwony (procent)")]
    [Range(0f, 1f)]
    public float lowTimeThreshold = 0.2f;

    void Start()
    {
        if (mapManager == null)
            mapManager = FindFirstObjectByType<MapManager>();
            
        if (timeText == null)
            timeText = GetComponent<TextMeshProUGUI>();
            
        if (timeText == null)
        {
            Debug.LogWarning("[MapTimeUI] No TextMeshProUGUI component found! Assign timeText.");
        }
        
        SetTextColor(normalColor);
        
        UpdateTimeDisplay();
    }

    void Update()
    {
        UpdateTimeDisplay();
    }

    void UpdateTimeDisplay()
    {
        if (mapManager == null)
        {
            SetText("No MapManager found!");
            return;
        }

        string displayText = string.Format(timeFormat, 
            mapManager.currentMapTime, 
            mapManager.maxMapTime);
            
        SetText(displayText);
        
        UpdateTextColor();
        
        CheckForBossTeleport();
    }
    
    void UpdateTextColor()
    {
        if (mapManager.maxMapTime <= 0f) return;
        
        float timeRatio = mapManager.currentMapTime / mapManager.maxMapTime;
        
        if (timeRatio <= lowTimeThreshold)
        {
            SetTextColor(lowTimeColor);
        }
        else
        {
            SetTextColor(normalColor);
        }
    }
    
    void CheckForBossTeleport()
    {
        if (mapManager.maxMapTime <= 0f) return;

        if (mapManager.currentMapTime <= 0f)
        {
            Debug.Log("[MapTimeUI] Time is up! Triggering boss teleport...");
            mapManager.TeleportToBossWhenTimeUp();
        }
    }
    
    void SetText(string text)
    {
        if (timeText != null)
            timeText.text = text;
    }
    
    void SetTextColor(Color color)
    {
        if (timeText != null)
            timeText.color = color;
    }
    
    [ContextMenu("Force Update Display")]
    public void ForceUpdateDisplay()
    {
        UpdateTimeDisplay();
    }
    
    public void SetTimeFormat(string newFormat)
    {
        timeFormat = newFormat;
        UpdateTimeDisplay();
    }
}
