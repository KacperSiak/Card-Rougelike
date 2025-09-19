using System;

[Serializable]
public class RestoreHPAfterRoom : PotionEffects
{
    private int localFightsLeft;
    private PotionContext context;
    public int healthToRestore;

    public override void Register(PotionContext ctx, int fightLeft)
    {
        localFightsLeft = fightLeft;
        context = ctx;
        RoundManager.OnCombatEnd += FightsLeft;
        RoundManager.OnCombatEnd += RestoreHP;
    }

    public override void Unregister(PotionContext ctx)
    {
        RoundManager.OnCombatEnd -= FightsLeft;
        RoundManager.OnCombatEnd -= RestoreHP;
    }

    public override void FightsLeft()
    {
        localFightsLeft -= 1;
        if (localFightsLeft == 0) Unregister(context);
    }

    private void RestoreHP()
    {
        PlayerManager.Instance.HealthChange(healthToRestore, false);
    }
}
