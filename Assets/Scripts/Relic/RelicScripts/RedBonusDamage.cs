using System;

[Serializable]
public class RedBonusDamage : RelicEffects
{
    private int _actuallDamage;
    private RelicContext _context;
    public override void Register(RelicContext ctx)
    {
        CardHolderLogic.OnRedPlay.AddListener(AddDamage);
        _context = ctx;
    }
    public override void Unregister(RelicContext ctx)
    {
        CardHolderLogic.OnRedPlay.RemoveAllListeners();
        ctx.damageManager.BonusDamageChange(_actuallDamage, true);
    }

    private void AddDamage()
    {
        _actuallDamage++;
        _context.damageManager.BonusDamageChange(1, true);
    }
}