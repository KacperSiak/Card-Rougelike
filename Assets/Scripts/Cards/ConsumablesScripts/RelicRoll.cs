using System;

[Serializable]
public class RelicRoll : CardEffect
{ 
    public override void ApplyEffect(ConsumableContext ctx)
    {
        int index = UnityEngine.Random.Range(0, ConsumablesAndRelicMenager.instance.currentRelics.Count);
        RelicEventsSystem.RelicRemoved(ctx.consAndRel.currentRelics[index]);
        // get one new after fight
    }
}
