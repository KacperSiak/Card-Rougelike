using UnityEngine;
using System;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace Systems.SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
    public static SceneLoader Instance { get; private set; }
        [SerializeField] Image loadingBar;
        [SerializeField] float fillSpeed = 0.5f;
        [SerializeField] Canvas loadingCanvas;
        [SerializeField] Camera loadingCamera;
    [SerializeField] SceneGroup[] sceneGroups;
    [SerializeField] float fadeHold = 0.15f; 

        float targetProgress;
        bool isLoading;
    bool missingFaderLogged;

        public readonly SceneGroupManager manager = new SceneGroupManager();

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            EnsureScreenFader();

            manager.OnSceneLoaded += sceneName => Debug.Log("Loaded: " + sceneName);
            manager.OnSceneUnloaded += sceneName => Debug.Log("Unloaded: " + sceneName);
            manager.OnSceneGroupLoaded += () => Debug.Log("Scene Group Loaded");
        }

        async void Start()
        {
            manager.RegisterSceneGroups(sceneGroups);
            await LoadSceneGroup(0);
        }

        void EnsureScreenFader()
        {
            if (ScreenFader.Instance != null) return;
            var go = new GameObject("ScreenFader");
            go.AddComponent<ScreenFader>();
        }

        void Update()
        {
            if (!isLoading) return;

            float currentFillAmount = loadingBar.fillAmount;
            float progressDifference = Mathf.Abs(currentFillAmount - targetProgress);

            float dynamicFillSpeed = progressDifference * fillSpeed;

            loadingBar.fillAmount = Mathf.Lerp(currentFillAmount, targetProgress, Time.deltaTime * dynamicFillSpeed);
        }

        public async Task LoadSceneGroup(int index)
        {
            loadingBar.fillAmount = 0f;
            targetProgress = 1f;

            if (index < 0 || index >= sceneGroups.Length)
            {
                Debug.LogError("Invalid scene group index: " + index);
                return;
            }

            LoadingProgress progress = new LoadingProgress();
            progress.Progressed += target => targetProgress = Mathf.Max(target, targetProgress);

            if (ScreenFader.Instance != null)
            {
                await ScreenFader.Instance.FadeOut();
                if (fadeHold > 0f) await Task.Delay(Mathf.RoundToInt(fadeHold * 1000f));
            }
            else if (!missingFaderLogged)
            {
                missingFaderLogged = true;
                Debug.LogWarning("[SceneLoader] FadeOut skipped: ScreenFader instance missing.");
            }

            EnableLoadingCanvas();
            await manager.LoadScenes(sceneGroups[index], progress);
            EnableLoadingCanvas(false);

            if (ScreenFader.Instance != null)
                await ScreenFader.Instance.FadeIn();
            else if (!missingFaderLogged)
            {
                missingFaderLogged = true;
                Debug.LogWarning("[SceneLoader] FadeIn skipped: ScreenFader instance missing.");
            }
        }

        public async Task ActivateSceneWithFade(string sceneName, bool loadIfMissing = true)
        {
            if (ScreenFader.Instance != null)
            {
                await ScreenFader.Instance.FadeOut();
                if (fadeHold > 0f) await Task.Delay(Mathf.RoundToInt(fadeHold * 1000f));
            }
            else if (!missingFaderLogged)
            {
                missingFaderLogged = true;
                Debug.LogWarning("[SceneLoader] FadeOut skipped: ScreenFader instance missing.");
            }

            await manager.ActivateScene(sceneName, loadIfMissing);

            if (ScreenFader.Instance != null)
                await ScreenFader.Instance.FadeIn();
            else if (!missingFaderLogged)
            {
                missingFaderLogged = true;
                Debug.LogWarning("[SceneLoader] FadeIn skipped: ScreenFader instance missing.");
            }
        }

        public void ActivateSceneFromNode(NodeClickHandler handler, string sceneName, string combatGuid)
        {
            _ = ActivateSceneFromNodeAsync(handler, sceneName, combatGuid);
        }

        private async Task ActivateSceneFromNodeAsync(NodeClickHandler handler, string sceneName, string combatGuid)
        {
            if (handler == null || string.IsNullOrEmpty(sceneName)) return;
            if (NodeClickHandler.forceDebugLogs || handler.enableDebugLogs)
                Debug.Log($"[SceneLoader] Node transition -> {sceneName} guid={combatGuid}");

            if (NodeClickHandler.IsCombatNode(handler.nodeType))
            {
                NodeClickHandler.lastEnteredCombatNode = handler;
                NodeClickHandler.SetLastEnteredCombatGuid(combatGuid);
                handler.DisableNodeOnCombatEntry(combatGuid);
            }

            await ActivateSceneWithFade(sceneName, true);

            if (NodeClickHandler.IsCombatNode(handler.nodeType))
            {
                CombatManager.ApplyNodeType(handler.nodeType);
            }
        }

        void EnableLoadingCanvas(bool enable = true)
        {
            isLoading = enable;
            
            if (loadingCanvas != null)
                loadingCanvas.gameObject.SetActive(enable);
                
            if (loadingCamera != null)
                loadingCamera.gameObject.SetActive(enable);
        }
    }
    public class LoadingProgress : IProgress<float>
    {
        public event Action<float> Progressed;

        const float ratio = 1f;

        public void Report(float value)
        {
            Progressed?.Invoke(value / ratio);
        }
    }
}
