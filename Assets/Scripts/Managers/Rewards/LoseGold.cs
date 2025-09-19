using System;

[Serializable]
public class LoseGold : Rewards
{
    public int gold;
    public override void AddReward(RewardContext ctx)
    {
        ctx.player.GoldChange(gold, false);
    }
}
