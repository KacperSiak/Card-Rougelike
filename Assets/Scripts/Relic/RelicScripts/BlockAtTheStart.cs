using System;

[Serializable]
public class BlockAtTheStart : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // prevent damage for first two turns
    }

    public override void Unregister(RelicContext ctx)
    {
        // do not work XD
    }
}

