using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Systems.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene Names")]
    [Tooltip("Name of the gameplay/map scene to load when starting a new game.")]
    public string mapSceneName = "MapGeneration";

    [Header("References")]
    [Tooltip("Assign MapSaveManager manually (no auto find). Can be left empty if you don't use saving yet.")]
    public MapSaveManager mapSaveManager;

    [Header("UI Panels (Optional)")]
    [Tooltip("Panel / root GameObject for options menu (can be toggled on/off). Optional.")]
    public GameObject optionsPanel;

    [Tooltip("Automatically hide options panel on start.")]
    public bool hideOptionsOnStart = true;

    void Start()
    {
        if (optionsPanel != null && hideOptionsOnStart)
            optionsPanel.SetActive(false);
    }

    public void OnNewGame()
    {
        if (string.IsNullOrEmpty(mapSceneName))
        {
            Debug.LogError("[MainMenuController] mapSceneName not set.");
            return;
        }

        if (MapGenerator.Instance != null)
        {
            MapGenerator.Instance.RegenerateNewSeed();
            return;
        }

        if (mapSaveManager != null)
        {
            mapSaveManager.DeleteSavedMap();
        }

        MapGenerator.ForceFreshGeneration = true;

        if (Systems.SceneManagement.SceneLoader.Instance != null)
        {
            _ = Systems.SceneManagement.SceneLoader.Instance.manager.ActivateScene(mapSceneName);
        }
        else
        {
            SceneManager.LoadScene(mapSceneName);
        }
    }

    public void OnContinueGame()
    {
        if (!HasSave())
        {
            Debug.Log("[MainMenuController] No save present for Continue.");
            return;
        }

        if (MapGenerator.Instance != null)
        {
            Debug.Log("[MainMenuController] Already in map context – ignoring continue.");
            return;
        }

        if (Systems.SceneManagement.SceneLoader.Instance != null)
        {
            _ = Systems.SceneManagement.SceneLoader.Instance.manager.ActivateScene(mapSceneName);
        }
        else
        {
            SceneManager.LoadScene(mapSceneName);
        }
    }

    public void OnOptions()
    {
        if (optionsPanel == null) return;
        optionsPanel.SetActive(true);
    }

    public void OnCloseOptions()
    {
        if (optionsPanel == null) return;
        optionsPanel.SetActive(false);
    }

    public bool HasSave()
    {
        return mapSaveManager != null && mapSaveManager.HasSavedMap();
    }

    public void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
