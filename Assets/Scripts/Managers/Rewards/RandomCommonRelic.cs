using System;

[Serializable]
public class RandomCommonRelic : Rewards
{
    private RelicData _rolledRelic;
    public override void AddReward(RewardContext ctx)
    {
        _rolledRelic = ctx.consAndRelic.RollRarityRelic(Rarity.Common);
        RelicEventsSystem.RelicAcquired(_rolledRelic);
    }
}
