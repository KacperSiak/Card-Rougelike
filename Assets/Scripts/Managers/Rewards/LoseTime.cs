using System;

[Serializable]  
public class LoseTime : Rewards
{
    public float timeToLose;

    public override void AddReward(RewardContext ctx)
    {
        ctx.mapManager.ModifieTime(timeToLose, false);
    }
}
