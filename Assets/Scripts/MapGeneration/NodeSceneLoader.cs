using UnityEngine;
using Systems.SceneManagement;

public static class NodeSceneLoader
{
    public static async System.Threading.Tasks.Task LoadFromNodeAsync(NodeClickHandler handler, string sceneToLoad, string combatGuid)
    {
        if (handler == null || string.IsNullOrEmpty(sceneToLoad)) return;

        if (NodeClickHandler.forceDebugLogs || handler.enableDebugLogs)
            Debug.Log($"[NodeSceneLoader] Loading scene '{sceneToLoad}' from node '{handler.name}' guid={combatGuid}");

        if (SceneLoader.Instance == null)
        {
            Debug.LogError("[NodeSceneLoader] SceneLoader.Instance missing. Ensure a SceneLoader object is present at startup.");
            return;
        }

        if (NodeClickHandler.IsCombatNode(handler.nodeType))
        {
            NodeClickHandler.lastEnteredCombatNode = handler;
            NodeClickHandler.SetLastEnteredCombatGuid(combatGuid);
            handler.DisableNodeOnCombatEntry(combatGuid);
        }

        await SceneLoader.Instance.manager.ActivateScene(sceneToLoad);
        if (NodeClickHandler.IsCombatNode(handler.nodeType))
        {
            CombatManager.ApplyNodeType(handler.nodeType);
        }
    }
    public static void LoadFromNode(NodeClickHandler handler, string sceneToLoad, string combatGuid)
    {
        _ = LoadFromNodeAsync(handler, sceneToLoad, combatGuid);
    }
}
