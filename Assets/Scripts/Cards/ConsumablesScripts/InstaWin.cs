using System;

[Serializable]
public class InstaWin : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        
        foreach (var relic in ctx.consAndRel.currentRelics)
        {
            RelicEventsSystem.RelicRemoved(relic);
        }
        // win fight (boss prevented)
    }
}
