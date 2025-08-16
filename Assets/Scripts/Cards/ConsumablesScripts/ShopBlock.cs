using System;

[Serializable]
public class ShopBlock : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        // blocks the shop
        for (int i = 0; i < 2; i++)
        {
            RelicData relic = ctx.consAndRel.RollRandomRelic();
            RelicEventsSystem.RelicAcquired(relic);
        }
    }
}
