using System;

[Serializable]
public class AddGold : Rewards
{
    private int goldAmount;
    public override void AddReward(RewardContext ctx)
    {
        ctx.player.AddGold(goldAmount);
    }
}
