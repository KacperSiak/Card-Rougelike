using System;

[Serializable]
public class BlackBonusDamage : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // plus damage for every played black card (no clue how to programm it XD)
    }
    public override void Unregister(RelicContext ctx)
    {
        // no more damage
    }
}

