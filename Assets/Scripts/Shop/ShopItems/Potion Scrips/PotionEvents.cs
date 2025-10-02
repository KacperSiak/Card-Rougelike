using Unity.Burst;
using Unity.VisualScripting;
using UnityEngine.Events;



public static class PotionEvents
{
    public static UnityEvent<Suits, bool> OnSuitProitection = new UnityEvent<Suits, bool>();
    public static UnityEvent<float, float, bool> OnShieldAcitvate = new UnityEvent<float, float, bool>();

    public static void ModifyProtection(Suits suit, bool add)
    {
        OnSuitProitection?.Invoke(suit, add);
    }

    public static void ModifyShield(float chances, float amount,  bool add)
    {
        OnShieldAcitvate?.Invoke(chances, amount, add);
    }
}
