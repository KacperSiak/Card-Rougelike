using System;

[Serializable]
public class EqualRedSuits : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        SuitRules.CombineRedSuits = true;
    }

    public override void Unregister(RelicContext ctx)
    {
        SuitRules.CombineRedSuits = false;
    }
}