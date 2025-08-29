using System;

[Serializable]
public class MaxHealth : Rewards
{
    public float newMaxHealth;
    public override void AddReward(RewardContext ctx)
    {
        ctx.player.SetMaxHP(newMaxHealth);
    }
}
