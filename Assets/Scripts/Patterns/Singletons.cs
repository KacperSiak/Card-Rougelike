using UnityEngine;

[DisallowMultipleComponent]
public abstract class PersistenSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }
    public static bool HasInstance => Instance != null;

    private static bool _isShuttingDown;

    [SerializeField]
    private bool persistAcrossScenes = false;
    protected virtual bool ShouldPersistAcrossScenes => persistAcrossScenes;

    protected virtual void Awake()
    {
        if (_isShuttingDown) return;

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this as T;
        if (ShouldPersistAcrossScenes)
        {
            DontDestroyOnLoad(gameObject);
        }
        OnSingletonAwake();
    }

    protected virtual void OnSingletonAwake() { }

    protected virtual void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void OnApplicationQuit()
    {
        _isShuttingDown = true;
    }
}
