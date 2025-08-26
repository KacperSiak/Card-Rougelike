using System;
using System.Diagnostics;

[Serializable]
public class CardPackReward : Rewards
{
    public CardPack pack;

    public override void AddReward(RewardContext ctx)
    {
        pack.OpenPack();
    }
}
