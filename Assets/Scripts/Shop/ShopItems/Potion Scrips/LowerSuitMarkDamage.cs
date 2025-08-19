using System;
using UnityEngine;

[Serializable]
public class LowerSuitMarkDamage : PotionEffects
{
    private int localFightsLeft;
    private PotionContext context;
    public override void Register(PotionContext ctx, int fightsLeft)
    {
        localFightsLeft = fightsLeft;
        context = ctx;
        RoundManager.OnCombatEnd += FightsLeft;
        Debug.Log("Protection Started");
        // Add lower damage from chosen suitmark
    }

    public override void Unregister(PotionContext ctx)
    {
        RoundManager.OnCombatEnd -= FightsLeft;
        Debug.Log("Protection Ended");
        // Remove protection form suitmark
    }

    public override void FightsLeft()
    {
        localFightsLeft -= 1;
        if (localFightsLeft == 0) Unregister(context);
    }
}
