using System;

[Serializable]
public class CardBuff : CardEffect
{
    public int boost;
    public override void ApplyEffect(ConsumableContext ctx)
    {
        // card damage x2/x3 (no idea if it works like 2 can be 4 in sequence after buff x2 or just damage i buffed)
    }
}
