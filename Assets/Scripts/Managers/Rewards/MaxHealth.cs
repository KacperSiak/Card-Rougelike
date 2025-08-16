using System;

[Serializable]
public class MaxHealth : Rewards
{
    private float newMaxHealth;
    public override void AddReward(RewardContext ctx)
    {
        ctx.player.SetMaxHP(newMaxHealth);
    }
}
