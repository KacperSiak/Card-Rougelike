using System;
using UnityEngine.Events;

[Serializable]
public class NoMoreHealsMoreHP: CardEffect
{
    public int bonusHealth;
    
    public override void ApplyEffect(ConsumableContext ctx)
    {
        ConsumablesEventSystem.SceneBlock("heal");
        ctx.player.MaxHPCHange(bonusHealth);
    }
}
