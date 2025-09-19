using System;

[Serializable]
public class BlockAtTheStart : RelicEffects
{
    private RelicContext _context;
    private int _counter = 2;
    public override void Register(RelicContext ctx)
    {
        _context = ctx;
        RoundManager.OnCombatStart += ResetRelic;
        RoundManager.OnRoundEnd += Count;
    }

    public override void Unregister(RelicContext ctx)
    {
        RoundManager.OnCombatStart -= ResetRelic;
        RoundManager.OnRoundEnd -= Count;
        ctx.roundManager.damageBlock = false;
    }

    private void ResetRelic()
    {
        _context.roundManager.damageBlock = true;
        _counter = 2;
    }

    private void Count()
    {
        _counter--;
        if (_counter == 0)
        {
            _context.roundManager.damageBlock = false; 
        }
    }
}

