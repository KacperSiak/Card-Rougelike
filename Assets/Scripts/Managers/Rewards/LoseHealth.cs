using System;
[Serializable]
public class LoseHealth : Rewards
{
    public int healthToLose;
    public override void AddReward(RewardContext ctx)
    {
        ctx.player.HealthChange(healthToLose, false);
    }
}
