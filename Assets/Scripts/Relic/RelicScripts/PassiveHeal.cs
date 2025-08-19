using System;

[Serializable]
public class PassiveHeal : RelicEffects
{
    public int healAmount;
    private RelicContext contex;
    public override void Register(RelicContext ctx)
    {
        contex = ctx;
        RoundManager.OnCombatEnd += Heal;
    }
    public override void Unregister(RelicContext ctx)
    {
        RoundManager.OnCombatEnd -= Heal;
    }

    void Heal()
    {
        contex.player.AddHealth(healAmount);
    }
}
