using System;

[Serializable]
public class AddDiscard : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        PlayerManager.Instance.availableDiscards++;

    }
    public override void Unregister(RelicContext ctx)
    {
        PlayerManager.Instance.availableDiscards--;
    }
}

