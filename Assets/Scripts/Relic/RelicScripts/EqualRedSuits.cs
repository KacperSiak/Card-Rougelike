using System;

[Serializable]
public class EqualRedSuits : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // hearths = diamonds
    }

    public override void Unregister(RelicContext ctx)
    {
        // hearths != diamonds
    }
}