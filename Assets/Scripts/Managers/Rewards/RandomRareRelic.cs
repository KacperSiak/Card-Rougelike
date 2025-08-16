using System;

[Serializable]
public class RandomRareRelic : Rewards
{
    private RelicData _rolledRelic;
    public override void AddReward(RewardContext ctx)
    {
        _rolledRelic = ctx.consAndRelic.RollRarityRelic(Rarity.Rare);
        RelicEventsSystem.RelicAcquired(_rolledRelic);
    }
}
