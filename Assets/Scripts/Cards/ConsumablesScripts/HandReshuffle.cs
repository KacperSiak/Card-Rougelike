using System;

[Serializable]
public class HandReshuffle : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        ctx.playerCardManager.Reshuffle();
    }
}
