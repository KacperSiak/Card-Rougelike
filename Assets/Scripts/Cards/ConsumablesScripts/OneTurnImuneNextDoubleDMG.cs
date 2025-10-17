using System;

[Serializable]
public class OneTurnImuneNextDoubleDMG : CardEffect
{
    private ConsumableContext _context;
    public override void ApplyEffect(ConsumableContext ctx)
    {
        _context = ctx;
        ctx.immune = true;
        RoundManager.OnRoundEnd += SwitchOffImune;
    }
    
    private void SwitchOffImune()
    {
        _context.immune = false;
        _context.vulnerable = true;
        RoundManager.OnRoundEnd -= SwitchOffImune;
        RoundManager.OnRoundEnd += SwitchOffDDamage;
    }

    private void SwitchOffDDamage()
    {
        _context.vulnerable = false;
        RoundManager.OnRoundEnd -= SwitchOffDDamage;
    }
}
