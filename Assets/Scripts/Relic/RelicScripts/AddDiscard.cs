using System;

[Serializable]
public class AddDiscard : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // +1 discard
    }
    public override void Unregister(RelicContext ctx)
    {
        // normal amount of discards
    }
}

