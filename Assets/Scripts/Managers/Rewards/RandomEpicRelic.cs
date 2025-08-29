using System;

[Serializable]
public class RandomEpicRelic : Rewards
{
    private RelicData _rolledRelic;
    public override void AddReward(RewardContext ctx)
    {
        _rolledRelic = ctx.consAndRelic.RollRarityRelic(Rarity.Epic);
        RelicEventsSystem.RelicAcquired(_rolledRelic);
    }
}
