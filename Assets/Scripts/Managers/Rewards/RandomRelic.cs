using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class RandomRelic : Rewards
{
    public RewardStruct[] rewards;

    public override void AddReward(RewardContext ctx)
    {
        int total = rewards.Sum(r => r.weight);
        int acc = 0;
        int random = UnityEngine.Random.Range(0, total);
        foreach (var r in rewards)
        {
            acc += r.weight;
            if (random < acc)
            {
                ApllyReward(r.reward);
            }
        }
    }

    private void ApllyReward(RewardsData reward)
    {
        var ctx = new RewardContext();
        foreach (var effect in reward.rewardEffect)
        {
            effect.AddReward(ctx);
        }
    }
}
