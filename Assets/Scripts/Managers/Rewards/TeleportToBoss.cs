using System;

[Serializable]
public class TeleportToBoss : Rewards
{
    public override void AddReward(RewardContext ctx)
    {
        ctx.mapManager.ModifyTime(ctx.mapManager.currentMapTime, false);
    }
}
