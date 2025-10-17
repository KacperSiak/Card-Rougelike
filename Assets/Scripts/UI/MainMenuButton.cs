using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using Systems.SceneManagement;
using UnityEngine.SceneManagement;

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
        {
            controller = GetComponentInParent<MainMenuController>();
            if (controller == null)
                controller = GetComponentInChildren<MainMenuController>(true);
        }
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
            if (!canContinue)
            {
                string latest = JsonSaveManager.GetLatestSavePath();
                canContinue = !string.IsNullOrEmpty(latest);
            }
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
                HandleContinue();
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
        if (_button != null) _button.interactable = false;
        JsonSaveManager.SetCurrentSlot("default");
        JsonSaveManager.SetMaxSavesPerSlot(10);
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
            if (_button != null) _button.interactable = true;
            return;
        }

        MapGenerator.ForceFreshGeneration = true;

        if (!string.IsNullOrEmpty(controller.mapSceneName))
        {
            var loader = SceneLoader.Instance;
            if (loader != null)
            {
                await loader.ActivateSceneWithFadeAndThen(controller.mapSceneName, async () =>
                {
                    float timeout = 5f;
                    while (MapGenerator.Instance == null && timeout > 0f)
                    {
                        timeout -= Time.unscaledDeltaTime;
                        await System.Threading.Tasks.Task.Yield();
                    }
                    if (MapGenerator.Instance != null)
                        MapGenerator.Instance.NewGameFresh();
                }, true);
                UnloadOwnScene();
                if (_button != null) _button.interactable = true;
            }
            else
            {
                if (ScreenFader.Instance != null) await ScreenFader.Instance.FadeOut();
                UnityEngine.SceneManagement.SceneManager.LoadScene(controller.mapSceneName);
                StartCoroutine(WaitAndFreshGenerate(withFinalFadeIn:true));
                if (_button != null) _button.interactable = true;
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
                yield return null;
                var _ = ScreenFader.Instance.FadeIn();
            }
        }
    }
    async void HandleContinue()
    {
        if (_button != null) _button.interactable = false;
        JsonSaveManager.SetCurrentSlot("default");
        string latest = JsonSaveManager.GetLatestSavePath();
        if (string.IsNullOrEmpty(latest))
        {
            Debug.Log("[MainMenuButton] No JSON save found.");
            if (_button != null) _button.interactable = true;
            return;
        }

        if (MapGenerator.Instance != null)
        {
            MapGenerator.Instance.TryLoadLatestJsonAndApply();
            if (_button != null) _button.interactable = true;
            return;
        }

        if (!string.IsNullOrEmpty(controller.mapSceneName))
        {
            var loader = Systems.SceneManagement.SceneLoader.Instance;
            if (loader != null)
            {
                await loader.ActivateSceneWithFadeAndThen(controller.mapSceneName, async () =>
                {
                    MapGenerator.Instance?.TryLoadLatestJsonAndApply();
                    await System.Threading.Tasks.Task.Yield();
                }, true);
                UnloadOwnScene();
                if (_button != null) _button.interactable = true;
            }
            else
            {
                if (ScreenFader.Instance != null) await ScreenFader.Instance.FadeOut();
                UnityEngine.SceneManagement.SceneManager.LoadScene(controller.mapSceneName);
                await System.Threading.Tasks.Task.Yield();
                MapGenerator.Instance?.TryLoadLatestJsonAndApply();
                if (ScreenFader.Instance != null) await ScreenFader.Instance.FadeIn();
                if (_button != null) _button.interactable = true;
            }
        }
        else
        {
            Debug.LogError("[MainMenuButton] mapSceneName not set on controller.");
            if (_button != null) _button.interactable = true;
        }
    }

    void UnloadOwnScene()
    {
        if (this == null || gameObject == null) return;
        var scene = gameObject.scene;
        if (scene.IsValid() && scene.isLoaded)
        {
            SceneManager.UnloadSceneAsync(scene);
        }
    }
}
