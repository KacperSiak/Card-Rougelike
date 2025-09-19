using System;

[Serializable]
public class MaxHealth : Rewards
{
    public int newMaxHealth;
    public override void AddReward(RewardContext ctx)
    {
        ctx.player.MaxHPCHange(newMaxHealth);
    }
}
