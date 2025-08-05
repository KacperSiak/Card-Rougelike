using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DebuffData", menuName = "Debuff/DebuffData")]
public class DebuffData : ScriptableObject
{
    public string debuffName;
    [TextArea] public string debuffDesc;
    public Sprite debuffIcon;
    public int debuffTurns;
    [SerializeReference] List<DebuffEffects> debuffs;
}

public class DebuffContext
{
    public PlayerOverlay player;
    public ShopMenager shopManager;

    public DebuffContext()
    {
        player = PlayerOverlay.instance;
        shopManager = ShopMenager.instance;
    }
}

[Serializable]
public abstract class DebuffEffects
{
    public abstract void Register(DebuffContext ctx, int turnCounter);
    public abstract void Unregister(DebuffContext ctx);
    public abstract void TurnsLeft();
}

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

[Serializable]
public class Reshuffle : DebuffEffects
{
    public override void Register(DebuffContext ctx, int turnCounter)
    {
        // reshuffle
    }

    public override void Unregister(DebuffContext ctx) {}
    public override void TurnsLeft() {}
}

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
        ctx.player.LoseHealth(lostHP);
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

[Serializable]
public class ShopPriceRise : DebuffEffects
{
    public float shopRise;
    public override void Register(DebuffContext ctx, int turnCounter)
    {
        ctx.shopManager.UpdatePrices(shopRise, false);
    }

    public override void Unregister(DebuffContext ctx)
    {
        // shop back
    }

    public override void TurnsLeft() {}
}

