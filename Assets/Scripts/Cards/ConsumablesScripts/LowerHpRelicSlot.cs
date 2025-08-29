using System;

[Serializable]
public class LowerHpRelicSlot : CardEffect
{
    public float hpModifier;
    public override void ApplyEffect(ConsumableContext ctx)
    {
        float newHP = (float)Math.Round(PlayerOverlay.Instance.maxHP * hpModifier);
        ctx.player.SetMaxHP(newHP);
        ctx.consAndRel.maxRelics += 1;
    }
}
