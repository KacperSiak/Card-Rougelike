using System;

[Serializable]
public class InstaWin : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        if (ctx.combbatManager.CurrentNodeType != MapSystem.NodeType.Boss)
        {
            foreach (var relic in ctx.consAndRel.currentRelics)
            {
                RelicEventsSystem.RelicRemoved(relic);
            }
            ConsumablesEventSystem.InstaWin();
        }
        else return;
        
    }
}
