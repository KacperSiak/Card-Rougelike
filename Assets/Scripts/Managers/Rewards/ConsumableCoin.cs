using System;
using UnityEngine;

[Serializable]
public class ConsumableCoin : Rewards
{
    public override void AddReward(RewardContext ctx)
    {
        int coin = UnityEngine.Random.Range(0, 2);
        if (coin == 0)
        {
            ConsumablesData cons = ctx.consAndRelic.RollRandomCons();
            ctx.consAndRelic.AddCard(cons);
        }
    }
}
