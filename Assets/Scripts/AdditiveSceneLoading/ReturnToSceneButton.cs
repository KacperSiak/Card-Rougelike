using UnityEngine;
using Systems.SceneManagement;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ReturnToSceneButton : MonoBehaviour
{
    [Tooltip("Nazwa sceny do aktywacji przez SceneLoader")] public string targetSceneName = "MapGeneration";
    private bool _isActivating;
    [Header("Widoczność")] 
    [SerializeField] bool hideIfAlreadyOnTarget = true;
    [SerializeField] string targetGroupName = "Nodes";
    [Tooltip("Sceny gdzie button ma być UKRYTY (blacklist)")]
    [SerializeField] string[] hideInScenes = new string[] { "MapGeneration", "VideoMainMenu", "MainMenu", "Bootstrapper" };
    [Header("Auto pozycja")] [SerializeField] bool autoPlaceBottomLeft = true;
    [SerializeField] Vector2 bottomLeftOffset = new Vector2(40f, 40f);
    CanvasGroup _cg;
    static ReturnToSceneButton _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.Log($"[ReturnToSceneButton] Duplicate found, destroying. Instance={_instance.gameObject.name}, This={gameObject.name}");
            Destroy(gameObject);
            return;
        }
        _instance = this;
        Debug.Log($"[ReturnToSceneButton] Awake: {gameObject.name} in scene {gameObject.scene.name}");
        EnsurePersistentCanvas();

        var btn = GetComponent<Button>();
        btn.onClick.RemoveListener(HandleClick);
        btn.onClick.AddListener(HandleClick);
        _cg = GetComponent<CanvasGroup>();
        if (_cg == null) _cg = gameObject.AddComponent<CanvasGroup>();

        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnActiveSceneChanged;
        RefreshVisibility(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        Debug.Log($"[ReturnToSceneButton] Awake complete. GameObject.activeSelf={gameObject.activeSelf}");
    }

    private void OnDestroy()
    {
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    void OnActiveSceneChanged(UnityEngine.SceneManagement.Scene oldScene, UnityEngine.SceneManagement.Scene newScene)
    {
        Debug.Log($"[ReturnToSceneButton] OnActiveSceneChanged: {oldScene.name} -> {newScene.name}");
        RefreshVisibility(newScene.name);
    }

    void RefreshVisibility(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("[ReturnToSceneButton] RefreshVisibility called with empty scene name");
            return;
        }
        
        bool allowed = true;
        
        if (hideInScenes != null && System.Array.Exists(hideInScenes, s => s == sceneName))
        {
            allowed = false;
        }

        if (_cg != null)
        {
            _cg.alpha = allowed ? 1f : 0f;
            _cg.blocksRaycasts = allowed;
            _cg.interactable = allowed;
        }
        
        gameObject.SetActive(allowed);
        
        Debug.Log($"[ReturnToSceneButton] RefreshVisibility scene={sceneName} allowed={allowed} gameObject.activeSelf={gameObject.activeSelf} parent={(transform.parent != null ? transform.parent.gameObject.name : "null")}");
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
        Canvas parentCanvas = null;
        var allCanvases = UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        foreach (var c in allCanvases)
        {
            if (c.gameObject.name == "ReturnButton_PersistentCanvas")
            {
                parentCanvas = c;
                Debug.Log($"[ReturnToSceneButton] Found existing ReturnButton canvas");
                break;
            }
        }

        if (parentCanvas == null)
        {
            var go = new GameObject("ReturnButton_PersistentCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 9999;
            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            DontDestroyOnLoad(go);
            parentCanvas = canvas;
            Debug.Log("[ReturnToSceneButton] Created NEW persistent canvas for button");
        }

        var rt = transform as RectTransform;
        if (rt == null) rt = gameObject.AddComponent<RectTransform>();
        
        if (rt.parent != parentCanvas.transform)
        {
            rt.SetParent(parentCanvas.transform, false);
            Debug.Log($"[ReturnToSceneButton] Button reparented to: {parentCanvas.gameObject.name}");
        }
        
        if (autoPlaceBottomLeft)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.zero;
            rt.pivot = Vector2.zero;
            rt.anchoredPosition = bottomLeftOffset;
            rt.sizeDelta = new Vector2(200, 80);
        }
    }

    void Update()
    {
        var currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (_lastCheckedScene != currentScene)
        {
            _lastCheckedScene = currentScene;
            RefreshVisibility(currentScene);
        }
        
        var parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null)
        {
            if (!parentCanvas.gameObject.activeSelf)
            {
                Debug.LogWarning($"[ReturnToSceneButton] Parent canvas DISABLED: {parentCanvas.gameObject.name}. Enabling!");
                parentCanvas.gameObject.SetActive(true);
            }
            if (!parentCanvas.enabled)
            {
                Debug.LogWarning($"[ReturnToSceneButton] Canvas component DISABLED: {parentCanvas.gameObject.name}. Enabling!");
                parentCanvas.enabled = true;
            }
        }
        else
        {
            Debug.LogError($"[ReturnToSceneButton] NO PARENT CANVAS FOUND! Button: {gameObject.name}");
        }
        
        if (!gameObject.activeSelf)
        {
            Debug.LogError($"[ReturnToSceneButton] Button DISABLED! Scene: {currentScene}");
        }
    }
    
    private string _lastCheckedScene = "";

    private IEnumerator ActivateRoutine()
    {
        _isActivating = true;
        TryAutosaveJson();
        var task = SceneLoader.Instance.ActivateSceneWithFade(targetSceneName, true);
        while (task != null && !task.IsCompleted)
            yield return null;
        _isActivating = false;
    }

    private void TryAutosaveJson()
    {
        try
        {
            var gen = MapGenerator.Instance;
            if (gen == null) return;
            var data = gen.BuildJsonSave();
            if (data == null) return;
            JsonSaveManager.Save(data);
            Debug.Log("[ReturnToSceneButton] Auto-saved JSON before returning to map");
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[ReturnToSceneButton] JSON autosave failed: {ex.Message}");
        }
    }
}
