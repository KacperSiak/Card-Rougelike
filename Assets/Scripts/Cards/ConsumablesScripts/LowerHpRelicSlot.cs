using System;

[Serializable]
public class LowerHpRelicSlot : CardEffect
{
    public float hpModifier;
    public override void ApplyEffect(ConsumableContext ctx)
    {
        float newHP = (float)Math.Round(PlayerOverlay.instance.maxHP * hpModifier);
        ctx.player.SetMaxHP(newHP);
        ctx.consAndRel.maxRelics += 1;
    }
}
