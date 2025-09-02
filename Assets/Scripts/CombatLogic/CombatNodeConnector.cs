using UnityEngine;

/// <summary>
/// Simple connector that sets up CombatManager based on node data
/// </summary>
public class CombatNodeConnector : MonoBehaviour
{
    void Start()
    {
        // Combat type is now automatically set in CombatManager.Awake()
        if (CombatManager.Instance != null)
        {
            Debug.Log($"[CombatNodeConnector] Combat setup complete for {CombatManager.Instance.CurrentNodeType}");
        }
        else
        {
            Debug.LogError("[CombatNodeConnector] CombatManager.Instance not found!");
        }
    }
}
