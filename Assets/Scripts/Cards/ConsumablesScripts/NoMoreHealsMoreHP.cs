using System;
using UnityEngine.Events;

[Serializable]
public class NoMoreHealsMoreHP: CardEffect
{
    public int bonusHealth;
    public static UnityEvent<string> OnShopBlock = new UnityEvent<string>();
    public override void ApplyEffect(ConsumableContext ctx)
    {
        OnShopBlock?.Invoke("Heal");
        ctx.player.MaxHPCHange(bonusHealth);
    }
}
