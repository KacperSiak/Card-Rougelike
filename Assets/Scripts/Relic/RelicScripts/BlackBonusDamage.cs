using System;

[Serializable]
public class BlackBonusDamage : RelicEffects
{
    private int _actuallDamage;
    private RelicContext _context;
    public override void Register(RelicContext ctx)
    {
        CardHolderLogic.OnBlackPlay.AddListener(AddDamage);
        _context = ctx;
    }
    public override void Unregister(RelicContext ctx)
    {
        CardHolderLogic.OnBlackPlay.RemoveAllListeners();
        ctx.damageManager.BonusDamageChange(_actuallDamage, true);
    }

    private void AddDamage()
    {
        _actuallDamage++;
        _context.damageManager.BonusDamageChange(1, true);
    }
}

