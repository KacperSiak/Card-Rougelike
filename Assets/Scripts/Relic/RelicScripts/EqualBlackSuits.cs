using System;

[Serializable]
public class EqualBlackSuits : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
    // spades = clubs
    }

    public override void Unregister(RelicContext ctx)
    {
    // spades != clubs
    }
}