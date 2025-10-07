using System;

[Serializable]
public class ShopPriceRise : DebuffEffects
{
    public float shopRise;
    public override void Register(DebuffContext ctx, int turnCounter)
    {
        ctx.shopManager.UpdatePrices(shopRise, false);
    }

    public override void Unregister(DebuffContext ctx)
    {
        ctx.shopManager.UpdatePrices(shopRise, true);
    }

    public override void TurnsLeft() {}
}

