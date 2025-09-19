using System;
using System.Diagnostics;

[Serializable]
public class AddGold : Rewards
{
    public int goldAmount;
    public override void AddReward(RewardContext ctx)
    {
        ctx.player.GoldChange(goldAmount, true);
    }
}
