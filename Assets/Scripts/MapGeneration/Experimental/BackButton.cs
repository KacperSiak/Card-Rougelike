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
            Debug.LogWarning("[BackButton] No Button component found!");
        }
    }
    
    public void GoBackToMap()
    {
        if (!string.IsNullOrEmpty(mapSceneName))
        {
            Debug.Log($"[BackButton] Loading map scene: {mapSceneName}");
            SceneManager.LoadScene(mapSceneName);
        }
        else
        {
            Debug.LogWarning("[BackButton] Map scene name is empty!");
        }
    }
    
    [ContextMenu("Test Go Back")]
    void TestGoBack()
    {
        GoBackToMap();
    }
    
    void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(GoBackToMap);
        }
    }
}
