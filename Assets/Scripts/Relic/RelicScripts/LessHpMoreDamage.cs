using System;

[Serializable]
public class LessHpMoreDamage : RelicEffects
{
    private RelicContext _context;
    private int _previousBonus;

    public int hpForBonus;
    public override void Register(RelicContext ctx)
    {
        _context = ctx;
        PlayerManager.OnHpChange.AddListener(AddDamage);
    }
    public override void Unregister(RelicContext ctx)
    {
        PlayerManager.OnHpChange.RemoveAllListeners();
    }

    private void AddDamage()
    {
        int lost = Math.Max(0, _context.player.MaxHP - _context.player.PlayerHP);
        int newBonus = lost / hpForBonus;
        int delta = newBonus - _previousBonus;

        if (delta != 0)
        {
            bool add = delta > 0;
            _context.damageManager.BonusDamageChange(Math.Abs(delta), add);
            _previousBonus = newBonus;
        }

    }
}