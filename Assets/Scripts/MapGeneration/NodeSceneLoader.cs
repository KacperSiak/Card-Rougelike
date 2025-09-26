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
        SceneLoader.Instance.ActivateSceneFromNode(handler, sceneToLoad, combatGuid);
    }
}
