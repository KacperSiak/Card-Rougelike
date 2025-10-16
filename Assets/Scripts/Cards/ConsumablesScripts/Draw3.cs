using System;

[Serializable]
public class Draw3 : CardEffect
{
    public int draw;
    public override void ApplyEffect(ConsumableContext ctx)
    {
        ctx.playerCardManager.Draw3Cards(draw);
    }
}
