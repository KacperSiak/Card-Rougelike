using UnityEngine;
using UnityEngine.Events;
public class PotionEventScript : MonoBehaviour
{
    public static UnityEvent<Suits> OnSuitProtection = new UnityEvent<Suits>();
    public static UnityEvent<Suits> OnSuitStopProtection = new UnityEvent<Suits>();

    public static void StartSuitProtect(Suits suit)
    {
        OnSuitProtection?.Invoke(suit);
    }

    public static void StopSuitProtection(Suits suit)
    {
        OnSuitStopProtection?.Invoke(suit);
    }
}
