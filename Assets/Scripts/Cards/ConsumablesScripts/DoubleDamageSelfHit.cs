using System;

[Serializable]
public class DoubleDamageSelfHit : CardEffect
{
    public int amount;
    private ConsumableContext _context;
    public override void ApplyEffect(ConsumableContext ctx)
    {
        _context = ctx;
        ctx.damageManager.MultiplyDamage(amount, true);
        ctx.selfHit = true;
        RoundManager.OnCombatEnd += Revert;
        // you get 10% damage of your attacks
    }

    private void Revert()
    {
        _context.damageManager.MultiplyDamage(amount, false);
        _context.selfHit = false;
        RoundManager.OnCombatEnd -= Revert;
    }
}
