using System;

[Serializable]
public class VaseReward : Rewards
{
    public VaseRelicUpgrade vase;
    public override void AddReward(RewardContext ctx)
    {
        vase.GotVase();
    }
}
