using System;

[Serializable]
public class Reshuffle : DebuffEffects
{
    public override void Register(DebuffContext ctx, int turnCounter)
    {
        // reshuffle
    }

    public override void Unregister(DebuffContext ctx) {}
    public override void TurnsLeft() {}
}

