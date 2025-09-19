using System;

[Serializable]
public class LowerHpRelicSlot : CardEffect
{
    public float hpModifier;
    public override void ApplyEffect(ConsumableContext ctx)
    {
        int newHP = (int)Math.Round(PlayerManager.Instance.PlayerHP * hpModifier);
        ctx.player.MaxHPCHange(newHP);
        ctx.consAndRel.maxRelics += 1;
    }
}
