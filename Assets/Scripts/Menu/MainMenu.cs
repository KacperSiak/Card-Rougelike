using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button exitButton;

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
        return;
    }

    private void ContinueGame()
    {
        return;
    }

    private void OpenOptions()
    {
        return;
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}
