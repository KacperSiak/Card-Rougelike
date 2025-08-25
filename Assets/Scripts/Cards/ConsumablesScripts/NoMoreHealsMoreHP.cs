using System;

[Serializable]
public class NoMoreHealsMoreHP: CardEffect
{
    public int bonusHealth;
    public override void ApplyEffect(ConsumableContext ctx)
    {
        // block fountains
        ctx.player.maxHP += bonusHealth;
        ctx.player.AddHealth(bonusHealth);
    }
}
