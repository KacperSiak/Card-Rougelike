using System;

[Serializable]
public class RedBonusDamage : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // plus damage for every played red card (no clue how to programm it XD)
    }
    public override void Unregister(RelicContext ctx)
    {
        // no more damage
    }
}