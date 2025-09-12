using System;

[Serializable]
public class HPLose : DebuffEffects
{
    private int localTurnCounter;
    private DebuffContext context;

    public int lostHP;
    public override void Register(DebuffContext ctx, int turnCounter)
    {
        localTurnCounter = turnCounter;
        context = ctx;
        RoundManager.OnRoundEnd += TurnsLeft;
        ctx.player.HealthChange(lostHP, false);
    }

    public override void Unregister(DebuffContext ctx)
    {
        RoundManager.OnRoundEnd -= TurnsLeft;
    }

    public override void TurnsLeft()
    {
        localTurnCounter -= 1;
        if (localTurnCounter <= 0) Unregister(context);
    }
}

