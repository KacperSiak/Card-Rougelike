using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PotionData", menuName = "Shop/PotionData")]
public class PotionData : ShopItem
{
    public string potionName;
    [TextArea] public string potionDescription;
    public int buyPrice;
    public int fightsLeft;
    public Sprite potionIcon;
    [SerializeReference] public List<PotionEffects> potionEffects;
}

public class PotionContext
{
    public PlayerOverlay player;
    public ShopMenager shopManager;

    public PotionContext()
    {
        player = PlayerOverlay.instance;
        shopManager = ShopMenager.instance;
    }
}

[Serializable]
public abstract class PotionEffects
{
    public abstract void Register(PotionContext ctx, int fightLeft);
    public abstract void Unregister(PotionContext ctx);
    public abstract void FightsLeft();
}

[Serializable]
public class Shield: PotionEffects
{
    private int localFightsLeft;
    private PotionContext context;
    public float chances;
    public float dmgReduction;
    public override void Register(PotionContext ctx, int fightsLeft)
    {
        localFightsLeft = fightsLeft;
        context = ctx;
        RoundManager.OnCombatEnd += FightsLeft;
        //AddShield
    }

    public override void Unregister(PotionContext ctx)
    {
        RoundManager.OnCombatEnd -= FightsLeft;
        // RemoveShield
    }
    public override void FightsLeft()
    {
        localFightsLeft -= 1;
        if (localFightsLeft == 0) Unregister(context);
    }
}

[Serializable]
public class LowerSuitMarkDamage : PotionEffects
{
    private int localFightsLeft;
    private PotionContext context;
    public override void Register(PotionContext ctx, int fightsLeft)
    {
        localFightsLeft = fightsLeft;
        context = ctx;
        RoundManager.OnCombatEnd += FightsLeft;
        // Add lower damage from chosen suitmark
    }

    public override void Unregister(PotionContext ctx)
    {
        RoundManager.OnCombatEnd -= FightsLeft;
        // Remove protection form suitmark
    }

    public override void FightsLeft()
    {
        localFightsLeft -= 1;
        if (localFightsLeft == 0) Unregister(context);
    }
}
[Serializable]
public class RemoveDebuff : PotionEffects
{
    public override void Register(PotionContext ctx, int fightLeft)
    {
        // Remove debuff
    }

    public override void Unregister(PotionContext ctx)
    {
        return;
    }

    public override void FightsLeft()
    {
        return;
    }
}

[Serializable]
public class DebuffProtection : PotionEffects
{
    private int localFightLeft;
    private PotionContext context;

    public override void Register(PotionContext ctx, int fightLeft)
    {
        localFightLeft = fightLeft;
        context = ctx;
        RoundManager.OnCombatEnd += FightsLeft;
        // debuff protection
    }

    public override void Unregister(PotionContext ctx)
    {
        RoundManager.OnCombatEnd -= FightsLeft;
        // debuff unprotection
    }

    public override void FightsLeft()
    {
        localFightLeft -= 1;
        if (localFightLeft == 0) Unregister(context);
    }
}

[Serializable]
public class RestoreHPAfterRoom : PotionEffects
{
    private int localFightsLeft;
    private PotionContext context;
    public int healthToRestore;

    public override void Register(PotionContext ctx, int fightLeft)
    {
        localFightsLeft = fightLeft;
        context = ctx;
        RoundManager.OnCombatEnd += FightsLeft;
        RoundManager.OnCombatEnd += RestoreHP;
    }

    public override void Unregister(PotionContext ctx)
    {
        RoundManager.OnCombatEnd -= FightsLeft;
        RoundManager.OnCombatEnd -= RestoreHP;
    }

    public override void FightsLeft()
    {
        localFightsLeft -= 1;
        if (localFightsLeft == 0) Unregister(context);
    }

    private void RestoreHP()
    {
        PlayerOverlay.instance.LoseHealth(healthToRestore);
    }
}

[Serializable]
public class MoreDamage : PotionEffects
{
    private int localFightsLeft;
    private PotionContext context;
    public float chances;
    public float bonusDamage;

    public override void Register(PotionContext ctx, int fightLeft)
    {
        localFightsLeft = fightLeft;
        context = ctx;
        RoundManager.OnCombatEnd += FightsLeft;
        // bonus damage
    }

    public override void Unregister(PotionContext ctx)
    {
        RoundManager.OnCombatEnd -= FightsLeft;
        // no more bons damage
    }

    public override void FightsLeft()
    {
        localFightsLeft -= 1;
        if (localFightsLeft == 0) Unregister(context);
    }
}

[Serializable]
public class BetterGamblingLuck : PotionEffects
{
    public override void Register(PotionContext ctx, int fightLeft)
    {
        // Better Gambling
    }

    public override void Unregister(PotionContext ctx)
    {
        // No more better luck
    }

    public override void FightsLeft()
    {
        return;
    }
}
