using System;

[Serializable]
public class SuitEnemyDamageBuff : DebuffEffects
{
    private int localTurnCounter;
    private DebuffContext context;

    public float bonusDamage;
    public override void Register(DebuffContext ctx, int turnCounter)
    {
        localTurnCounter = turnCounter;
        context = ctx;
        RoundManager.OnRoundEnd += TurnsLeft;
        // get bonus damage
    }

    public override void Unregister(DebuffContext ctx)
    {
        RoundManager.OnRoundEnd -= TurnsLeft;
        // no more bonus damage
    }

    public override void TurnsLeft()
    {
        localTurnCounter -= 1;
        if (localTurnCounter <= 0)
            Unregister(context);
    }
}

