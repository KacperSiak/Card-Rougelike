using System;

[Serializable]
public class NoMoreHealsMoreHP: CardEffect
{
    public int bonusHealth;
    public override void ApplyEffect(ConsumableContext ctx)
    {
        // block fountains
        ctx.player.MaxHPCHange(bonusHealth);
        ctx.player.HealthChange(bonusHealth, true);
    }
}
