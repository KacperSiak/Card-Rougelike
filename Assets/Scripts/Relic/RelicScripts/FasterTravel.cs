using System;

[Serializable]
public class FasterTravel : RelicEffects
{
    public float speedModifier;
    public override void Register(RelicContext ctx)
    {
        ctx.mapManager.SetNewModifier(speedModifier);
    }

    public override void Unregister(RelicContext ctx)
    {
        ctx.mapManager.ResetModifier();
    }
}