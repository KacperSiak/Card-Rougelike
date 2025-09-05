using System;

[Serializable]
public class LoseHpGetDamage : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        int _hpLose = (int)Math.Round(PlayerManager.Instance.PlayerHP * 0.25f);
        ctx.player.HealthChange(_hpLose, false);
        //player.addDamage = 5;
    }
}
