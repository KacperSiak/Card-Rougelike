using System;

[Serializable]
public class RemoveDebuff : PotionEffects
{
    public override void Register(PotionContext ctx, int fightLeft)
    {
        ctx.buffsAndDebuffsManager.RemoveRandomDebuff();
    }

    public override void Unregister(PotionContext ctx)
    {
        return;
    }

    public override void FightsLeft()
    {
        return;
    }
}
