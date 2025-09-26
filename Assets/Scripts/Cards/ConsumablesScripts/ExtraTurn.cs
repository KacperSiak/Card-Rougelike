using System;

[Serializable]
public class ExtraTurn : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        ctx.bonusRound = true;
    }
}
