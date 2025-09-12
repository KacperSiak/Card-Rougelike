using UnityEngine;
using Systems.SceneManagement;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ReturnToSceneButton : MonoBehaviour
{
    [Tooltip("Nazwa sceny do aktywacji przez SceneLoader")] public string targetSceneName = "MapGeneration";
    private bool _isActivating;

    private void Awake()
    {
        var btn = GetComponent<Button>();
        btn.onClick.RemoveListener(HandleClick);
        btn.onClick.AddListener(HandleClick);
    }

    private void HandleClick()
    {
        if (_isActivating) return;
        if (string.IsNullOrEmpty(targetSceneName)) { Debug.LogWarning("[ReturnToSceneButton] Brak nazwy sceny"); return; }
        if (SceneLoader.Instance == null) { Debug.LogWarning("[ReturnToSceneButton] Brak SceneLoader.Instance"); return; }
        StartCoroutine(ActivateRoutine());
    }

    private IEnumerator ActivateRoutine()
    {
        _isActivating = true;
        var task = SceneLoader.Instance.manager.ActivateScene(targetSceneName);
        while (task != null && !task.IsCompleted)
            yield return null;
        _isActivating = false;
    }
}
