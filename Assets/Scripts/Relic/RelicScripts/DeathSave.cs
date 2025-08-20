using System;

[Serializable]
public class DeathSave : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // death protection self destruct on activation
    }

    public override void Unregister(RelicContext ctx)
    {
        // you will die :)
    }
}