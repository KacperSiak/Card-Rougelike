using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles returning to map scene after combat
/// </summary>
public class ReturnToMapHandler : MonoBehaviour
{
    [Header("Scene Settings")]
    public string mapSceneName = "MapScene";
    
    [Header("Debug")]
    public bool debugMode = true;
    
    /// <summary>
    /// Call this to return to map after combat ends
    /// </summary>
    public void ReturnToMap()
    {
        if (debugMode) Debug.Log("[ReturnToMapHandler] Returning to map scene");
        
        // Clear combat data (now handled by static variable)
        CombatManager.NextCombatNodeType = MapSystem.NodeType.Enemy;
        
        // Load map scene
        if (!string.IsNullOrEmpty(mapSceneName))
        {
            SceneManager.LoadScene(mapSceneName);
        }
        else
        {
            Debug.LogError("[ReturnToMapHandler] No map scene name specified");
        }
    }
    
    /// <summary>
    /// Call this for testing in editor
    /// </summary>
    [ContextMenu("Return To Map")]
    public void TestReturnToMap()
    {
        ReturnToMap();
    }
}
