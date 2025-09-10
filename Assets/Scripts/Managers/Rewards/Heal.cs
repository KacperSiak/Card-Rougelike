using System;

[Serializable]
public class Heal : Rewards
{
    private int healAmount; 
    public override void AddReward(RewardContext ctx)
    {
        ctx.player.HealthChange(healAmount, true);
    }
}
