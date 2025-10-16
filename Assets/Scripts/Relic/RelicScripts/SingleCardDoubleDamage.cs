using System;

[Serializable]
public class SingleCardDoubleDamage : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        ctx.doubleSingleDamage = true;
    }

    public override void Unregister(RelicContext ctx)
    {
        ctx.doubleSingleDamage = false;
    }
}
