using System;

[Serializable]
public class LoseHpGetDamage : CardEffect
{
    public int bonusDamage;
    private ConsumableContext _context;
    public override void ApplyEffect(ConsumableContext ctx)
    {
        _context = ctx;
        int _hpLose = (int)Math.Round(PlayerManager.Instance.PlayerHP * 0.25f);
        ctx.player.HealthChange(_hpLose, false);
        ctx.damageManager.BonusDamageChange(bonusDamage, true);
        RoundManager.OnCombatEnd += RevertDamage;
    }

    private void RevertDamage()
    {
        _context.damageManager.BonusDamageChange(bonusDamage, false);
        RoundManager.OnCombatEnd -= RevertDamage;
    }
}
