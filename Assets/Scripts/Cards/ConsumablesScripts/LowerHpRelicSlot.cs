using System;

[Serializable]
public class LowerHpRelicSlot : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        float newHP = (float)Math.Round(PlayerOverlay.instance.maxHP * 0.8f);
        ctx.player.SetMaxHP(newHP);
        ctx.consAndRel.maxRelics += 1;
    }
}
