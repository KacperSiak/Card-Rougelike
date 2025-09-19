using System;

[Serializable]
public class StartWithAdditionalDamage : RelicEffects
{
    private RelicContext _context;
    public int damage;

    public override void Register(RelicContext ctx)
    {
        _context = ctx;
        RoundManager.OnCombatStart += SetDamage;
    }
    public override void Unregister(RelicContext ctx)
    {
        RoundManager.OnCombatStart -= SetDamage;
    }

    private void SetDamage()
    {
        _context.damageManager.BonusDamageChange(damage, true);
        RoundManager.OnRoundEnd += RemoveDamage;
    }

    private void RemoveDamage()
    {
        _context.damageManager.BonusDamageChange(damage, false);
        RoundManager.OnRoundEnd -= RemoveDamage;
    }
}
