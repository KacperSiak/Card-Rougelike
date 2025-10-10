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
    [Tooltip("Format wyświetlania czasu. Używa {0} i {1} jako miejsc na tekst czasu.")]
    public string timeFormat = "Time: {0} / {1}";
    [Tooltip("Kolor tekstu gdy czas się kończy (czerwony)")]
    public Color lowTimeColor = Color.red;
    [Tooltip("Normalny kolor tekstu")]
    public Color normalColor = Color.white;
    [Tooltip("Próg poniżej którego tekst staje się czerwony (procent)")]
    [Range(0f, 1f)]
    public float lowTimeThreshold = 0.2f;

    [Header("Hours/Minutes Display")]
    [Tooltip("Czy wyświetlać czas jako godziny:minuty zamiast surowych jednostek.")]
    public bool showAsHoursMinutes = true;
    [Tooltip("Ile minut odpowiada jednej jednostce czasu (np. 12 => 80 jednostek = 16h)")]
    public float minutesPerUnit = 12f;
    [Tooltip("Czy dopełniać minuty zerem wiodącym (np. 5 -> 05)")]
    public bool padMinutes = true;

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
        
        if (timeText != null)
            timeText.color = normalColor;
        
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

        if (showAsHoursMinutes && mapManager != null && mapManager.minutesPerUnit > 0f)
        {
            minutesPerUnit = mapManager.minutesPerUnit;
        }

        string displayText;
        if (showAsHoursMinutes && minutesPerUnit > 0f)
        {
            string cur = FormatAsHoursMinutes(mapManager.currentMapTime);
            string max = FormatAsHoursMinutes(mapManager.maxMapTime);
            displayText = string.Format(timeFormat, cur, max);
        }
        else
        {
            displayText = string.Format(timeFormat,
                mapManager.currentMapTime.ToString("F1"),
                mapManager.maxMapTime.ToString("F1"));
        }
            
        SetText(displayText);
        
        UpdateTextColor();
        
        CheckForBossTeleport();
    }
    
    void UpdateTextColor()
    {
        if (mapManager.maxMapTime <= 0f) return;
        
        float timeRatio = mapManager.currentMapTime / mapManager.maxMapTime;
        
        if (timeText != null)
        {
            timeText.color = (timeRatio <= lowTimeThreshold) ? lowTimeColor : normalColor;
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
    
    // Removed SetTextColor helper to avoid naming conflicts and keep it simple.
    
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

    string FormatAsHoursMinutes(float units)
    {
        if (units < 0f) units = 0f;
        float totalMinutesF = units * minutesPerUnit;
        int totalMinutes = Mathf.RoundToInt(totalMinutesF);
        int hours = totalMinutes / 60;
        int minutes = Mathf.Abs(totalMinutes % 60);
        if (padMinutes)
            return $"{hours}:{minutes:00}h";
        else
            return $"{hours}:{minutes}h";
    }
}
