using System;
using UnityEngine.Events;

[Serializable]
public class Discount : RelicEffects
{
    public float discount;

    public static UnityEvent<float> OnDiscountActivated = new();
    public static UnityEvent OnDiscountDeactivated = new();
    public override void Register(RelicContext ctx)
    {
        OnDiscountActivated?.Invoke(discount);
    }
    public override void Unregister(RelicContext ctx)
    {
        OnDiscountDeactivated?.Invoke();
    }
}
