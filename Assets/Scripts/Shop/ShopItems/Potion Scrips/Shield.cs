using System;

[Serializable]
public class Shield : PotionEffects
{
    private int localFightsLeft;
    private PotionContext context;
    public float chances;
    public float dmgReduction;
    public override void Register(PotionContext ctx, int fightsLeft)
    {
        localFightsLeft = fightsLeft;
        context = ctx;
        RoundManager.OnCombatEnd += FightsLeft;
        PotionEvents.ModifyShield(chances, dmgReduction, true);
    }

    public override void Unregister(PotionContext ctx)
    {
        RoundManager.OnCombatEnd -= FightsLeft;
        PotionEvents.ModifyShield(chances, dmgReduction, false);
    }
    public override void FightsLeft()
    {
        localFightsLeft -= 1;
        if (localFightsLeft == 0) Unregister(context);
    }
}
