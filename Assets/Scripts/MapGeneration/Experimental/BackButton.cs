using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
    [Header("Scene Settings")]
    public string mapSceneName = "MapScene";
    
    [Header("Auto Setup")]
    public bool autoAttachToButton = true;
    
    private Button button;

    void Start()
    {
        if (autoAttachToButton)
        {
            SetupButton();
        }
    }
    
    void SetupButton()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(GoBackToMap);
        }
        else
        {
            Debug.LogWarning("No Button component found on BackButton GameObject");
        }
    }
    
    public void GoBackToMap()
    {
        if (!string.IsNullOrEmpty(mapSceneName))
        {
            SceneManager.LoadScene(mapSceneName);
        }
        else
        {
            Debug.LogWarning("Map scene name not specified");
        }
    }
    
    public void GoBackToMapEvent()
    {
        GoBackToMap();
    }
}
