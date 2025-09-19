using System;

[Serializable]
public class Handsize : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        ctx.player.handsize++;
    }
    public override void Unregister(RelicContext ctx)
    {
        ctx.player.handsize--;
    }
}