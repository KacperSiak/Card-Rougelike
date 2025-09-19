using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] Button settingsButton;
    [SerializeField] Button backToMainMenuButton;
    [SerializeField] Button exitGameButton;

    [Space()]
    [Header("GameObjects")]
    [SerializeField] GameObject holderObject;
    [SerializeField] GameObject settingsPanelObject;

    [Space()]
    [SerializeField] string mainMenuSceneName;

    private void OnEnable()
    {
        settingsButton.onClick.AddListener(OpenSettings);
        backToMainMenuButton.onClick.AddListener(BackToMainMenu);
        exitGameButton.onClick.AddListener(ExitGame);
    }

    private void OnDisable()
    {
        settingsButton.onClick.RemoveAllListeners();
        backToMainMenuButton.onClick.RemoveAllListeners();
        exitGameButton.onClick.RemoveAllListeners();
    }


    public void OpenSettings()
    {
        settingsPanelObject.SetActive(true);
        holderObject.SetActive(false);
    }

    public void BackToMainMenu()
    {
        // Save game here

        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void ExitGame()
    {
        // Save game here

        Application.Quit();
    }
}
