using System;

[Serializable]
public class BonusGold : RelicEffects
{
    public float bonusGold;
    public override void Register(RelicContext ctx)
    {
        // bonus gold
    }

    public override void Unregister(RelicContext ctx)
    {
        //no more bonus gold
    }
}

