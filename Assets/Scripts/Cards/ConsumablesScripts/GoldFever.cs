using System;

[Serializable]
public class GoldFever : CardEffect
{
    public int gold;
    public override void ApplyEffect(ConsumableContext ctx)
    {
        ctx.player.HealthChange(gold, true);
    }
}
