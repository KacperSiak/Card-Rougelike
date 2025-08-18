using System;

[Serializable]
public class Handsize : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // handsize +1
    }
    public override void Unregister(RelicContext ctx)
    {
        // normal handsize
    }
}