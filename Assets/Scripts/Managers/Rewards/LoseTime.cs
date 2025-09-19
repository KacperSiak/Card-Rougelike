using System;

[Serializable]  
public class LoseTime : Rewards
{
    public float timeToLose;

    public override void AddReward(RewardContext ctx)
    {
        ctx.mapManager.ModifyTime(timeToLose, false);
    }
}
