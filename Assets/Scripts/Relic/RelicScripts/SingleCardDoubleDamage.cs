using System;

[Serializable]
public class SingleCardDoubleDamage : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // beating a sequence with a single card deasls Double damage
    }

    public override void Unregister(RelicContext ctx)
    {
        // no more double damage
    }
}
