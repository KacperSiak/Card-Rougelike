using System;
using UnityEngine;

[Serializable]
public class LowerSuitMarkDamage : PotionEffects
{
    private int localFightsLeft;
    private PotionContext context;

    public Suits suit;
    public override void Register(PotionContext ctx, int fightsLeft)
    {
        localFightsLeft = fightsLeft;
        context = ctx;
        RoundManager.OnCombatEnd += FightsLeft;
        PotionEvents.ModifyProtection(suit, true);
    }

    public override void Unregister(PotionContext ctx)
    {
        RoundManager.OnCombatEnd -= FightsLeft;
        PotionEvents.ModifyProtection(suit, false);
    }

    public override void FightsLeft()
    {
        localFightsLeft -= 1;
        if (localFightsLeft == 0) Unregister(context);
    }
}
