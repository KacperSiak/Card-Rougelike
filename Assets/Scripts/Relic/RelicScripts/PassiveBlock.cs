using System;

[Serializable]
public class PassiveBlock : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // block passive for 2 turns
    }

    public override void Unregister(RelicContext ctx)
    {
        // no more blocks
    }
}
