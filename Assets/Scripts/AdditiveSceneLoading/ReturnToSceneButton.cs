using UnityEngine;
using Systems.SceneManagement;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ReturnToSceneButton : MonoBehaviour
{
    [Tooltip("Nazwa sceny do aktywacji przez SceneLoader")] public string targetSceneName = "MapGeneration";
    private bool _isActivating;
    [Header("Widoczność")] [SerializeField] bool hideIfAlreadyOnTarget = true;
    [SerializeField] string targetGroupName = "Nodes"; // nazwa grupy w SceneLoader
    [Header("Auto pozycja")] [SerializeField] bool autoPlaceBottomLeft = true;
    [SerializeField] Vector2 bottomLeftOffset = new Vector2(40f, 40f);
    CanvasGroup _cg;
    static ReturnToSceneButton _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        EnsurePersistentCanvas();

        var btn = GetComponent<Button>();
        btn.onClick.RemoveListener(HandleClick);
        btn.onClick.AddListener(HandleClick);
        _cg = GetComponent<CanvasGroup>();
        if (_cg == null) _cg = gameObject.AddComponent<CanvasGroup>();

        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnActiveSceneChanged;
        RefreshVisibility(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    void OnActiveSceneChanged(UnityEngine.SceneManagement.Scene oldScene, UnityEngine.SceneManagement.Scene newScene)
    {
        RefreshVisibility(newScene.name);
    }

    void RefreshVisibility(string sceneName)
    {
        bool allowed = false;
        var loader = Systems.SceneManagement.SceneLoader.Instance;
        if (loader != null)
        {
            allowed = loader.IsSceneInGroup(sceneName, targetGroupName);
        }
        if (hideIfAlreadyOnTarget && sceneName == targetSceneName) allowed = false;

        if (_cg != null)
        {
            _cg.alpha = allowed ? 1f : 0f;
            _cg.blocksRaycasts = allowed;
            _cg.interactable = allowed;
        }
        else
        {
            gameObject.SetActive(allowed);
        }
        Debug.Log($"[ReturnToSceneButton] RefreshVisibility scene={sceneName} allowed={allowed}");
    }

    private void HandleClick()
    {
        if (_isActivating) return;
        if (string.IsNullOrEmpty(targetSceneName)) { Debug.LogWarning("[ReturnToSceneButton] Brak nazwy sceny"); return; }
        if (SceneLoader.Instance == null) { Debug.LogWarning("[ReturnToSceneButton] Brak SceneLoader.Instance"); return; }
        Debug.Log($"[ReturnToSceneButton] Click -> target='{targetSceneName}'");
        StartCoroutine(ActivateRoutine());
    }

    void EnsurePersistentCanvas()
    {
        // If already under a DontDestroyOnLoad parent, skip
        if (gameObject.scene.name == "DontDestroyOnLoad")
            return;

        Canvas parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null && parentCanvas.gameObject.scene.name == "DontDestroyOnLoad")
        {
            DontDestroyOnLoad(parentCanvas.gameObject);
            return;
        }

        // Find existing persistent canvas
        var allCanvases = FindObjectsOfType<Canvas>(true);
        foreach (var c in allCanvases)
        {
            if (c.gameObject.scene.name == "DontDestroyOnLoad" && c.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                parentCanvas = c;
                break;
            }
        }

        if (parentCanvas == null)
        {
            var go = new GameObject("PersistentUI_Canvas_Manager", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            DontDestroyOnLoad(go);
            parentCanvas = canvas;
            Debug.Log("[ReturnToSceneButton] Created persistent canvas");
        }
        else
        {
            DontDestroyOnLoad(parentCanvas.gameObject);
        }

        var rt = transform as RectTransform;
        if (rt == null) rt = gameObject.AddComponent<RectTransform>();
        rt.SetParent(parentCanvas.transform, false);
        if (autoPlaceBottomLeft)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.zero;
            rt.pivot = Vector2.zero;
            rt.anchoredPosition = bottomLeftOffset;
        }
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        var parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null && !parentCanvas.gameObject.activeSelf)
        {
            parentCanvas.gameObject.SetActive(true);
        }
        if (parentCanvas != null && !parentCanvas.enabled)
            parentCanvas.enabled = true;
    }

    private IEnumerator ActivateRoutine()
    {
        _isActivating = true;
        var task = SceneLoader.Instance.ActivateSceneWithFade(targetSceneName, true);
        while (task != null && !task.IsCompleted)
            yield return null;
        _isActivating = false;
    }
}
