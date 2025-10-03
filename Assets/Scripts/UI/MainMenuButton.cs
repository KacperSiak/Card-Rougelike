using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using Systems.SceneManagement; // for ScreenFader & SceneLoader

[RequireComponent(typeof(Button))]
public class MainMenuButton : MonoBehaviour, IPointerClickHandler
{
    public enum MenuButtonType
    {
        NewGame,
        Continue,
        Options,
        Quit
    }

    public MenuButtonType buttonType = MenuButtonType.NewGame;
    [Tooltip("Assign the MainMenuController in the scene.")]
    public MainMenuController controller;
    [Tooltip("If true, will auto-find controller on Awake if not assigned (same scene).")]
    public bool autoFindController = true;

    Button _button;

    void Awake()
    {
        _button = GetComponent<Button>();
        if (controller == null && autoFindController)
            controller = Object.FindFirstObjectByType<MainMenuController>();
    }

    void Start()
    {
        RefreshState();
    }

    public void RefreshState()
    {
        if (_button == null) return;
        if (buttonType == MenuButtonType.Continue)
        {
            bool canContinue = controller != null && controller.HasSave();
            _button.interactable = canContinue;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (controller == null)
        {
            Debug.LogWarning("[MainMenuButton] No MainMenuController assigned.");
            return;
        }

        switch (buttonType)
        {
            case MenuButtonType.NewGame:
                HandleNewGame();
                break;
            case MenuButtonType.Continue:
                controller.OnContinueGame();
                break;
            case MenuButtonType.Options:
                controller.OnOptions();
                break;
            case MenuButtonType.Quit:
                controller.OnQuit();
                break;
        }

        RefreshState();
    }

    async void HandleNewGame()
    {
        // If already on the map, fade -> regenerate -> fade in.
        if (MapGenerator.Instance != null)
        {
            if (ScreenFader.Instance != null)
            {
                await ScreenFader.Instance.FadeOut();
            }
            MapGenerator.Instance.NewGameFresh();
            if (ScreenFader.Instance != null)
            {
                await ScreenFader.Instance.FadeIn();
            }
            return;
        }

        // Otherwise force a fresh generation once map scene loads.
        MapGenerator.ForceFreshGeneration = true;

        if (!string.IsNullOrEmpty(controller.mapSceneName))
        {
            var loader = SceneLoader.Instance;
            if (loader != null)
            {
                // Use existing fade-capable API so we don't duplicate logic.
                await loader.ActivateSceneWithFade(controller.mapSceneName, true);
                // After activation (and while screen is faded back or finishing), ensure regeneration.
                StartCoroutine(WaitAndFreshGenerate());
            }
            else
            {
                // Manual fallback: fade if possible, load, then wait and regenerate.
                if (ScreenFader.Instance != null) await ScreenFader.Instance.FadeOut();
                UnityEngine.SceneManagement.SceneManager.LoadScene(controller.mapSceneName);
                StartCoroutine(WaitAndFreshGenerate(withFinalFadeIn:true));
            }
        }
    }

    IEnumerator WaitAndFreshGenerate(bool withFinalFadeIn = false)
    {
        float timeout = 5f;
        while (MapGenerator.Instance == null && timeout > 0f)
        {
            timeout -= Time.unscaledDeltaTime;
            yield return null;
        }
        if (MapGenerator.Instance != null)
        {
            MapGenerator.Instance.NewGameFresh();
            if (withFinalFadeIn && ScreenFader.Instance != null)
            {
                // Delay one frame to allow objects to spawn before fade-in.
                yield return null;
                var _ = ScreenFader.Instance.FadeIn();
            }
        }
    }
}
