using System;

[Serializable]
public class BetterGamblingLuck : PotionEffects
{
    // public float modifier;

    // private float resetModifier = 1;
    public override void Register(PotionContext ctx, int fightLeft)
    {
        // ctx.gambler.SetModifier(modifier);
        // GamblingManager.OnGamblingExit.AddListener(ResetModifier);
    }

    public override void Unregister(PotionContext ctx){}

    public override void FightsLeft(){}

    private void ResetModifier()
    {
        // GamblingManager.instance.SetModifier(resetModifier);
    }

}
