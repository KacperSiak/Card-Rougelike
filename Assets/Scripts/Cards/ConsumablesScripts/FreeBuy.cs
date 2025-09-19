using System;

[Serializable]
public class FreeBuy : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        ctx.shop.ActiveFreBuy();
    }
}
