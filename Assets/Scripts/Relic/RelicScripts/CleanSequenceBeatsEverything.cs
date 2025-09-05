using System;
using UnityEngine.Events;

[Serializable]
public class CleanSequenceBeatsEverything : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        ctx.CleanSequenceBeatEverythingEnable = true;
    }

    public override void Unregister(RelicContext ctx)
    {
        ctx.CleanSequenceBeatEverythingEnable = false;
    }
}