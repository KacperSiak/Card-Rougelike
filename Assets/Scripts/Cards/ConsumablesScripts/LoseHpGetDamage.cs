using System;

[Serializable]
public class LoseHpGetDamage : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        int _hpLose = (int)Math.Round(PlayerOverlay.instance.currentHP * 0.25f);
        ctx.player.LoseHealth(_hpLose);
        //player.addDamage = 5;
    }
}
