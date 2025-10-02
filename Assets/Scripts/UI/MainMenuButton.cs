using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
            controller = FindObjectOfType<MainMenuController>();
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
                controller.OnNewGame();
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
}
