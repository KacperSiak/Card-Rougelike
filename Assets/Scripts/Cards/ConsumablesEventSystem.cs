using UnityEngine;
using UnityEngine.Events;

public class ConsumablesEventSystem : MonoBehaviour
{
    public static UnityEvent<string> OnSceneBlock = new UnityEvent<string>();
    public static UnityEvent OnInstaWin = new UnityEvent();
    public static void SceneBlock(string name)
    {
        OnSceneBlock?.Invoke(name);
    }

    public static void InstaWin()
    {
        OnInstaWin?.Invoke();
    }
}
