using System;

[Serializable]
public class OnTheEdge : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        ctx.player.maxHP = 5;
        ctx.player.currentHP = 5;
        // ctx.player.SetMaxHP(5)
        // after 2 turn going back to previous hp
        // immune to debuffs
    }
}
