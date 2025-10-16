using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyboardListener : MonoBehaviour
{
    [SerializeField] GameObject pauseGameObject;
    KeyCode pauseKey = KeyCode.Escape;

    private void Start()
    {
        pauseGameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey))
            Pause();
    }

    public void Pause()
    {
        Debug.Log(SceneManager.GetActiveScene().name);
        switch (SceneManager.GetActiveScene().name)
        {
            case "GamblingScene":
                if (GamblingSceneManager.Instance.isIdle)
                {
                    pauseGameObject.SetActive(!pauseGameObject.activeInHierarchy);
                    Time.timeScale = pauseGameObject.activeInHierarchy ? 0 : 1;
                }
                break;

            default:
                pauseGameObject.SetActive(!pauseGameObject.activeInHierarchy);
                Time.timeScale = pauseGameObject.activeInHierarchy ? 0 : 1;
                break;
        }
    }
}
