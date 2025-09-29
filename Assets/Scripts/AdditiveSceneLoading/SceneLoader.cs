using UnityEngine;
using System;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

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

            RefreshEventSystem();

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
            var active = SceneManager.GetActiveScene().name;
            if (active == sceneName)
            {
                // Same scene – just visual pulse (no reload) and EventSystem refresh.
                await PulseFade();
                RefreshEventSystem();
                return;
            }

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

            RefreshEventSystem();

            if (ScreenFader.Instance != null)
                await ScreenFader.Instance.FadeIn();
            else if (!missingFaderLogged)
            {
                missingFaderLogged = true;
                Debug.LogWarning("[SceneLoader] FadeIn skipped: ScreenFader instance missing.");
            }
        }

        public async Task PulseFade(float outDuration = -1f, float inDuration = -1f)
        {
            if (ScreenFader.Instance == null) return;
            await ScreenFader.Instance.FadeOut(outDuration);
            if (fadeHold > 0f) await Task.Delay(Mathf.RoundToInt(fadeHold * 1000f));
            await ScreenFader.Instance.FadeIn(inDuration);
        }

        public bool IsSceneInGroup(string sceneName, string groupName)
        {
            if (string.IsNullOrEmpty(sceneName) || string.IsNullOrEmpty(groupName) || sceneGroups == null) return false;
            for (int g = 0; g < sceneGroups.Length; g++)
            {
                var grp = sceneGroups[g];
                if (grp == null || grp.GroupName != groupName) continue;
                if (grp.Scenes == null) continue;
                for (int s = 0; s < grp.Scenes.Count; s++)
                {
                    var data = grp.Scenes[s];
                    if (data != null && data.Name == sceneName) return true;
                }
            }
            return false;
        }

        void RefreshEventSystem()
        {
            // Destroy existing EventSystem (if any) and create a fresh one to clear stale references / selection.
            var existing = GameObject.FindObjectOfType<EventSystem>();
            if (existing != null)
            {
                Destroy(existing.gameObject);
            }
            var go = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            DontDestroyOnLoad(go); // Keep consistent between additive scenes until next refresh.
            // Optional: clear selected object
            var es = go.GetComponent<EventSystem>();
            if (es != null)
            {
                es.SetSelectedGameObject(null);
            }
            Debug.Log("[SceneLoader] EventSystem refreshed.");
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
