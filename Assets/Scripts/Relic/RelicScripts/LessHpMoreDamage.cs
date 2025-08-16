using System;

[Serializable]
public class LessHpMoreDamage : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // +1 damage for every 2 missing health
    }
    public override void Unregister(RelicContext ctx)
    {
        // no more additional damage
    }
}