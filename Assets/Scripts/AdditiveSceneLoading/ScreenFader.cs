using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace Systems.SceneManagement
{
    public class ScreenFader : MonoBehaviour
    {
        public static ScreenFader Instance { get; private set; }

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image overlayImage;
    [SerializeField] private float defaultDuration = 0.4f;
    [SerializeField] private bool autoFadeInOnStart = true;
    [SerializeField] private bool verboseLogs = true;
    [SerializeField] private bool keepCanvasEnabled = true;
    Canvas cachedCanvas;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (canvasGroup == null)
            {
                var c = gameObject.GetComponent<Canvas>();
                if (c == null) c = gameObject.AddComponent<Canvas>();
                c.renderMode = RenderMode.ScreenSpaceOverlay;
                c.sortingOrder = 5000;
                if (gameObject.GetComponent<GraphicRaycaster>() == null)
                    gameObject.AddComponent<GraphicRaycaster>();

                canvasGroup = gameObject.GetComponent<CanvasGroup>();
                if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
                if (overlayImage == null)
                {
                    var imgGO = new GameObject("Overlay");
                    imgGO.transform.SetParent(transform, false);
                    overlayImage = imgGO.AddComponent<Image>();
                    overlayImage.color = Color.black;
                    var rt = overlayImage.rectTransform;
                    rt.anchorMin = Vector2.zero;
                    rt.anchorMax = Vector2.one;
                    rt.offsetMin = Vector2.zero;
                    rt.offsetMax = Vector2.zero;
                }
            }
            canvasGroup.alpha = 1f;
        }

        async void Start()
        {
            if (autoFadeInOnStart)
            {
                await FadeIn(defaultDuration);
            }
        }

        void Update()
        {
            if (keepCanvasEnabled)
            {
                if (cachedCanvas == null)
                    cachedCanvas = GetComponent<Canvas>();
                if (cachedCanvas != null && !cachedCanvas.enabled)
                {
                    cachedCanvas.enabled = true;
                    if (verboseLogs) Debug.Log("[ScreenFader] Canvas was disabled externally -> re-enabled");
                }
            }
        }

        public Task FadeOut(float duration = -1f) => Fade(1f, duration);
        public Task FadeIn(float duration = -1f) => Fade(0f, duration);

        async Task Fade(float target, float duration)
        {
            if (duration <= 0f) duration = defaultDuration;
            if (canvasGroup == null)
            {
                Debug.LogWarning("ScreenFader: canvasGroup missing, reinitializing.");
                Awake();
            }
            float start = canvasGroup.alpha;
            float t = 0f;
            canvasGroup.blocksRaycasts = true;
            if (verboseLogs) Debug.Log($"[ScreenFader] Fade start from {start:F2} to {target:F2} dur={duration:F2}");
            while (t < 1f)
            {
                t += Time.unscaledDeltaTime / duration;
                canvasGroup.alpha = Mathf.Lerp(start, target, t);
                await Task.Yield();
            }
            canvasGroup.alpha = target;
            canvasGroup.blocksRaycasts = target > 0.01f;
            if (verboseLogs) Debug.Log($"[ScreenFader] Fade end alpha={canvasGroup.alpha:F2}");

        }


        public static void EnsureInstance()
        {
            if (Instance != null) return;
            var go = new GameObject("ScreenFader");
            go.AddComponent<ScreenFader>();
        }
    }
}
