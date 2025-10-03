using System;

[Serializable]
public class MoreDamage : PotionEffects
{
    private int localFightsLeft;
    private PotionContext context;
    public float chances;
    public float bonusDamage;

    public override void Register(PotionContext ctx, int fightLeft)
    {
        localFightsLeft = fightLeft;
        context = ctx;
        RoundManager.OnCombatEnd += FightsLeft;
        ctx.damageManager.MultiplyDamage(bonusDamage, true);
    }

    public override void Unregister(PotionContext ctx)
    {
        RoundManager.OnCombatEnd -= FightsLeft;
        ctx.damageManager.MultiplyDamage(bonusDamage, false);
    }

    public override void FightsLeft()
    {
        localFightsLeft -= 1;
        if (localFightsLeft == 0) Unregister(context);
    }
}
