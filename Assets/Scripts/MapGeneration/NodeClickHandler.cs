using UnityEngine;
using UnityEngine.SceneManagement;

public class NodeClickHandler : MonoBehaviour
{
    [Header("Scene To Load")]
    public string sceneToLoad = "EnemyScene";

    void Start()
    {
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }
    }

    void OnMouseDown()
    {
        Debug.Log($"Clicked on node - loading scene: {sceneToLoad}");
        
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("No scene name specified!");
        }
    }
}
