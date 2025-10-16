using UnityEngine;
using Systems.SceneManagement;

public static class NodeSceneLoader
{
    public static void LoadFromNode(NodeClickHandler handler, string sceneToLoad, string combatGuid)
    {
        if (SceneLoader.Instance == null)
        {
            Debug.LogError("[NodeSceneLoader] SceneLoader.Instance missing. Ensure a SceneLoader object is present at startup.");
            return;
        }
        try
        {
            bool isCombat = NodeClickHandler.IsCombatNode(handler != null ? handler.nodeType : MapSystem.NodeType.Enemy);
            if (!isCombat)
            {
                var gen = MapGenerator.Instance;
                if (gen != null)
                {
                    var data = gen.BuildJsonSave();
                    if (data != null)
                    {
                        JsonSaveManager.Save(data);
                        Debug.Log("[NodeSceneLoader] Auto-saved JSON before entering non-combat scene");
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[NodeSceneLoader] JSON autosave failed: {ex.Message}");
        }
        SceneLoader.Instance.ActivateSceneFromNode(handler, sceneToLoad, combatGuid);
    }
}
