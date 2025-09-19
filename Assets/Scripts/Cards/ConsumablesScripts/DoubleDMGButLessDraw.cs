using System;

[Serializable]
public class DoubleDMGButLessDraw : CardEffect
{
    private ConsumableContext _context;
    public int amount;
    public override void ApplyEffect(ConsumableContext ctx)
    {
        _context.damageManager.MultiplyDamage(amount, false);
        ctx.drawBlock = true;
        RoundManager.OnRoundEnd += SwitchOffDoubleDamage;
    }

    private void SwitchOffDoubleDamage()
    {
        _context.damageManager.MultiplyDamage(amount, false);
        _context.drawBlock = false;
        RoundManager.OnRoundEnd -= SwitchOffDoubleDamage;
    }
}
