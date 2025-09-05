using System;

[Serializable]
public class AddDiscard : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // PlayerOverlay.Instance.discardCount++;

    }
    public override void Unregister(RelicContext ctx)
    {
        // PlayerOverlay.Instance.discardCount--;
    }
}

