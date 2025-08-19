using System;

[Serializable]
public class AddTime : Rewards
{
    private float time;
    public override void AddReward(RewardContext ctx)
    {
        ctx.mapManager.ModifyTime(time, true);
    }
}
