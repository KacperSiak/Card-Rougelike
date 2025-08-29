using System;

[Serializable]
public class HealForCoins : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        int gold = ctx.player.gold;
        ctx.player.SpendGold(gold);
        ctx.player.AddHealth(gold);
    }
}
