using System;

[Serializable]
public class CleanSequenceBeatsEverything : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // Clean sequence can beat every minion
    }

    public override void Unregister(RelicContext ctx)
    {
        // no more ultrasequence
    }
}