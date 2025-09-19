using System;

[Serializable]
public class EqualBlackSuits : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        SuitRules.CombineBlackSuits = true;
    }

    public override void Unregister(RelicContext ctx)
    {
        SuitRules.CombineBlackSuits = false;
    }
}