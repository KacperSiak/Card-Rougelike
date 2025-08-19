using System;

[Serializable]
public class StartWithAdditionalDamage : RelicEffects
{
    public int damage;
    public override void Register(RelicContext ctx)
    {
        // +damage for first hit 
    }
    public override void Unregister(RelicContext ctx)
    {
        // no more additional damage at the start of the combat
    }
}
