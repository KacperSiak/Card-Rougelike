using System;

[Serializable]
public class SlowerMinions : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // minion + turn
    }
    public override void Unregister(RelicContext ctx)
    {
        // minion normal turn
    }
}
