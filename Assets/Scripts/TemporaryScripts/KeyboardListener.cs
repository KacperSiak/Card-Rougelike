using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyboardListener : MonoBehaviour
{
    [SerializeField] GameObject pauseGameObject;
    KeyCode pauseKey = KeyCode.Escape;

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey))
            Pause();
    }

    public void Pause()
    {
        pauseGameObject.SetActive(!pauseGameObject.activeInHierarchy);
    }
}
