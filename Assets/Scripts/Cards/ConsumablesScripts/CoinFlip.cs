using System;

[Serializable]
public class CoinFlip : CardEffect
{
    public int gold;
    public override void ApplyEffect(ConsumableContext ctx)
    {
        int coin = UnityEngine.Random.Range(0, 2);
        if (coin == 0)
        {
            ctx.player.AddGold(gold);
        }
    }
}
