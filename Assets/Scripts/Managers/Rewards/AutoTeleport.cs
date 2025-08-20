using System;

[Serializable]
public class AutoTeleport : Rewards
{
    public override void AddReward(RewardContext ctx)
    {
        ctx.mapManager.AutoTeleport();
    }
}