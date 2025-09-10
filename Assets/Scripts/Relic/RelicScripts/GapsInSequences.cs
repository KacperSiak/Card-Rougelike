using System;

[Serializable]
public class GapsInSequences : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        ctx.Gaps = true;
    }

    public override void Unregister(RelicContext ctx)
    {
        ctx.Gaps = false;
    }
}