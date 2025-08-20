using System;
using UnityEngine;

[Serializable]  
public class LoseRandomRelic : Rewards
{
    public override void AddReward(RewardContext ctx)
    {
        RelicData relic = ctx.consAndRelic.currentRelics[UnityEngine.Random.Range(0, ctx.consAndRelic.currentRelics.Count)];
        RelicEventsSystem.RelicRemoved(relic);
    }
}
