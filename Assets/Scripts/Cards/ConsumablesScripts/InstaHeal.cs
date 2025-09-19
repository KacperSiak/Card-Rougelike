using System;

[Serializable]
public class InstaHeal : CardEffect
{
    public int healh;
    public override void ApplyEffect(ConsumableContext ctx)
    {
        ctx.player.HealthChange(healh, true); 
    }
}
