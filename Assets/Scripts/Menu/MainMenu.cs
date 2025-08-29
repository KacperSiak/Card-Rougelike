using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button exitButton;

    [SerializeField] private GameObject optionsPanel;
    private bool isOptionsOpen = false;

    [Header("Game Settings")]
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private MapSaveDataSO defaultSaveTemplate;

    private void OnEnable()
    {
        newGameButton.onClick.AddListener(StartNewGame);
        loadGameButton.onClick.AddListener(ContinueGame);
        optionButton.onClick.AddListener(OpenOptions);
        exitButton.onClick.AddListener(ExitGame);
    }

    private void OnDisable()
    {
        newGameButton.onClick.RemoveAllListeners();
        loadGameButton.onClick.RemoveAllListeners();
        optionButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();
    }
    
    private void StartNewGame()
    {
        if (GameSaveManager.Instance != null)
        {
            GameSaveManager.Instance.CreateNewGameSave(defaultSaveTemplate);
        }
        
        if (!string.IsNullOrEmpty(gameSceneName))
        {
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            Debug.LogWarning("[MainMenu] Game scene name not set!");
        }
    }

    private void ContinueGame()
    {
        return;
    }

    private void OpenOptions()
    {
        isOptionsOpen = !isOptionsOpen;
        optionsPanel.SetActive(isOptionsOpen);
    }

    [Header("List of windows in settings panel")]
    [SerializeField] private GameObject[] windows;
    public void HideAllWindows()
    {
        foreach(GameObject window in windows)
            window.SetActive(false);
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}
