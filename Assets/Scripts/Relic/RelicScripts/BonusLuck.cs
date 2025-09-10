using System;

[Serializable]
public class BonusLuck : RelicEffects
{
    public float modifier;
    public override void Register(RelicContext ctx)
    {
        //ctx.gambler.Modify(modifier, false, false);
    }
    public override void Unregister(RelicContext ctx)
    {
        //ctx.gambler.Modify(modifier, false, true);
    }
}