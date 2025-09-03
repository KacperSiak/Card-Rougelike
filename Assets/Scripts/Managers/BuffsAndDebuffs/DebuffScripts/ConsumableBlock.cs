using System;

[Serializable]
public class ConsumableBlock : DebuffEffects
{
    private int localTurnCounter;
    private DebuffContext context;

    public override void Register(DebuffContext ctx, int turncounter)
    {
        localTurnCounter = turncounter;
        context = ctx;
        RoundManager.OnRoundEnd += TurnsLeft;
        RoundManager.OnCombatEnd += TurnOff;
        // block of consumablecrds
    }

    public override void Unregister(DebuffContext ctx)
    {
        RoundManager.OnRoundEnd -= TurnsLeft;
        RoundManager.OnCombatEnd -= TurnOff;
        // unlock consumables
    }
    
    public override void TurnsLeft()
    {
        localTurnCounter--;
        if (localTurnCounter <= 0)
            Unregister(context);
    }

    public void TurnOff()
    {
        localTurnCounter = 0;
        TurnsLeft();
    }
}

