using System;

[Serializable]
public class BonusCardPackReward : Rewards
{
    public CardPack cardPack;
    public override void AddReward(RewardContext ctx)
    {
        cardPack.OpenPack();
    }
}
