using System;

[Serializable]
public class AddTime : Rewards
{
    public float time;
    public override void AddReward(RewardContext ctx)
    {
        ctx.mapManager.ModifieTime(time, true);
    }
}
