using System;

[Serializable]
public class ShopBlock : CardEffect
{
    public int relicCount;
    public override void ApplyEffect(ConsumableContext ctx)
    {
        // blocks the shop
        for (int i = 0; i < relicCount; i++)
        {
            RelicData relic = ctx.consAndRel.RollRandomRelic();
            RelicEventsSystem.RelicAcquired(relic);
        }
    }
}
