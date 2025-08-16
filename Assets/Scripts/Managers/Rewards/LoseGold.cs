using System;

[Serializable]
public class LoseGold : Rewards
{
    private int gold;
    public override void AddReward(RewardContext ctx)
    {
        ctx.player.SpendGold(gold);
    }
}
