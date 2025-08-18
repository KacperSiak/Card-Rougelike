using System;

[Serializable]
public class DebuffProtection : PotionEffects
{
    private int localFightLeft;
    private PotionContext context;

    public override void Register(PotionContext ctx, int fightLeft)
    {
        localFightLeft = fightLeft;
        context = ctx;
        RoundManager.OnCombatEnd += FightsLeft;
        ctx.buffsAndDebuffsManager.DebuffProtectionEnable();
    }

    public override void Unregister(PotionContext ctx)
    {
        RoundManager.OnCombatEnd -= FightsLeft;
        ctx.buffsAndDebuffsManager.DebuffProtectionDisable();
    }

    public override void FightsLeft()
    {
        localFightLeft -= 1;
        if (localFightLeft == 0) Unregister(context);
    }
}
