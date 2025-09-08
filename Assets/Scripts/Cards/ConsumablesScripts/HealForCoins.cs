using System;

[Serializable]
public class HealForCoins : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        int gold = ctx.player.gold;
        ctx.player.GoldChange(gold, false);
        ctx.player.HealthChange(gold, true);
    }
}
