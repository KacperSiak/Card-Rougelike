using System;

[Serializable]
public class BonusLuck : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // better rng
    }
    public override void Unregister(RelicContext ctx)
    {
        // Normal rng
    }
}