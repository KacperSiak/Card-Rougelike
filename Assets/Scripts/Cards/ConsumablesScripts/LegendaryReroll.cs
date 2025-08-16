using System;

[Serializable]
public class LegendaryReroll : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        foreach (var relic in ctx.consAndRel.currentRelics)
        {
            RelicEventsSystem.RelicRemoved(relic);
        }
        for (int i = 0; i < 2; i++)
        {
            RelicData relic = ctx.consAndRel.RollRarityRelic(Rarity.Legendary);
            RelicEventsSystem.RelicAcquired(relic);
        }
    }
}
