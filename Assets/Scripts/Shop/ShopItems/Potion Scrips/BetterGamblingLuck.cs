using System;

[Serializable]
public class BetterGamblingLuck : PotionEffects
{
    public float newModifier;
    public override void Register(PotionContext ctx, int fightLeft)
    {
        ctx.gambler.Modify(newModifier);
    }

    public override void Unregister(PotionContext ctx){}

    public override void FightsLeft(){}
}
