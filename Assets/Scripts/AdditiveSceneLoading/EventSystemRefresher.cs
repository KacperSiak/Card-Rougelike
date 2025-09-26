using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class EventSystemRefresher : MonoBehaviour
{
    private static EventSystemRefresher _instance;
    public static bool EnableLogs = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoInstall()
    {
        if (_instance != null) return;
        var go = new GameObject("EventSystemRefresher");
        _instance = go.AddComponent<EventSystemRefresher>();
        DontDestroyOnLoad(go);
    }

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        StartCoroutine(RefreshNextFrame(newScene));
    }

    private IEnumerator RefreshNextFrame(Scene scene)
    {
        yield return null;

        EventSystem target = null;
    var systems = Object.FindObjectsByType<EventSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var es in systems)
        {
            if (es != null && es.gameObject.scene == scene)
            {
                target = es;
                break;
            }
        }

        if (target == null)
        {
            var go = new GameObject("EventSystem (Auto)");
            SceneManager.MoveGameObjectToScene(go, scene);
            target = go.AddComponent<EventSystem>();
            go.AddComponent<StandaloneInputModule>();
            if (EnableLogs) Debug.Log("[EventSystemRefresher] Created EventSystem in scene: " + scene.name);
        }

        if (target.isActiveAndEnabled)
        {
            target.enabled = false;
            if (EnableLogs) Debug.Log("[EventSystemRefresher] Disable EventSystem");
            yield return null; // one more frame
            target.enabled = true;
            if (EnableLogs) Debug.Log("[EventSystemRefresher] Enable EventSystem");
        }
        else
        {
            target.enabled = true;
            if (EnableLogs) Debug.Log("[EventSystemRefresher] Enabled previously disabled EventSystem");
        }

        target.SetSelectedGameObject(null);
    }
}
