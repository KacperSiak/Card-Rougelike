using System;

[Serializable]
public class RandomLegendaryRelic : Rewards
{
    private RelicData _rolledRelic;
    public override void AddReward(RewardContext ctx)
    {
        _rolledRelic = ctx.consAndRelic.RollRarityRelic(Rarity.Legendary);
        RelicEventsSystem.RelicAcquired(_rolledRelic);
    }
}
