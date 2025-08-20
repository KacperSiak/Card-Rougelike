using System;

[Serializable]
public class NoMoreHealsMoreHP: CardEffect
{
    private int bonusHealth;
    public override void ApplyEffect(ConsumableContext ctx)
    {
        // block fountains
        ctx.player.maxHP += bonusHealth;
        ctx.player.AddHealth(bonusHealth);
    }
}
