using System;

[Serializable]
public class HugeReroll : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        ctx.consAndRel.ReplaceAllConsumables();
    }
}
