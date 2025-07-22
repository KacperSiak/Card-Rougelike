using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;


public enum Rarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

[CreateAssetMenu(fileName = "RelicData", menuName = "Shop/Relic")]
public class RelicData : ShopItem
{
    public string relicID;
    public string relicName;
    [TextArea] public string description;
    public Sprite icon;
    public Rarity rarity;
    [SerializeReference] public List<RelicEffects> relicEffets;
}

public class RelicContext
{
    public PlayerOverlay player;
}


[Serializable]
public abstract class RelicEffects
{
    public abstract void Register(RelicContext ctx);
    public abstract void Unregister(RelicContext ctx);
}

[Serializable]
public class BlackIsBlack : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
    // spades = clubs
    }

    public override void Unregister(RelicContext ctx)
    {
    // spades != clubs
    }
}

[Serializable]
public class RedIsRed : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // hearths = diamonds
    }

    public override void Unregister(RelicContext ctx)
    {
        // hearths != diamonds
    }
}

[Serializable]
public class Gaps : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // sequences gap
    }

    public override void Unregister(RelicContext ctx)
    {
        // no more sequences gap
    }
}

[Serializable]
public class HealingSequence : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
    // healing 
    }

    public override void Unregister(RelicContext ctx)
    {
    // no more healing 
    }
}

[Serializable]
public class Interest : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // interest++
    }

    public override void Unregister(RelicContext ctx)
    {
        // interest come back to 5
    }
}

[Serializable]
public class HearthPlus : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // hearths damage
    }

    public override void Unregister(RelicContext ctx)
    {
        // hearths damage no more
    }
}

[Serializable]
public class DiamnodPlus : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // Diamonds damage
    }

    public override void Unregister(RelicContext ctx)
    {
        // diamonds damage no more
    }
}

[Serializable]
public class ClubsPlus : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // clubs damage
    }

    public override void Unregister(RelicContext ctx)
    {
        // clubs damage no more
    }
}

[Serializable]
public class SpadesPlus : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // spades damage
    }

    public override void Unregister(RelicContext ctx)
    {
        // spades damage no more
    }
}

[Serializable]
public class DeathSave : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // death protection self destruct on activation
    }

    public override void Unregister(RelicContext ctx)
    {
        // you will die :)
    }
}

[Serializable]
public class PowerStrike : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // additional damage
    }

    public override void Unregister(RelicContext ctx)
    {
        // no more additional damage
    }
}

[Serializable]
public class FasterThenLight : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // faster travel
    }

    public override void Unregister(RelicContext ctx)
    {
        // normal travel
    }
}

[Serializable]
public class BetterStart : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // prevent damage irst two turns
    }

    public override void Unregister(RelicContext ctx)
    {
        // do not work XD
    }
}

[Serializable]
public class UnclearDraw : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // unclear sequence -> draw
    }

    public override void Unregister(RelicContext ctx)
    {
            
    }
}

[Serializable]
public class BetterDamage : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // block over 3 ----> +3
    }
    public override void Unregister(RelicContext ctx)
    {
            
    }
}

[Serializable]
public class AddDiscard : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // +1 discard
    }
    public override void Unregister(RelicContext ctx)
    {
        // normal amount of discards
    }
}

[Serializable]
public class SameSuit : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
  
        // additional damage ifg suit is the same
    }
    public override void Unregister(RelicContext ctx)
    {
        // nothing
    }
}

[Serializable]
public class AllForAll : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // every sequence can beat every sequence
    }

    public override void Unregister(RelicContext ctx)
    {
            
    }
}

[Serializable]
public class DiscardOrGold : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // +gold for discard
    }

    public override void Unregister(RelicContext ctx)
    {
        // no more gold for discard
    }
}

[Serializable]
public class DoubleDamage : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // double damage
    }
    public override void Unregister(RelicContext ctx)
    {
        // normal damage
    }
}

[Serializable]
public class Handsize : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // handsize +1
    }
    public override void Unregister(RelicContext ctx)
    {
        // normal handsize
    }
}

[Serializable]
public class ZeroDiscardBoost : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // +1 damage if 0 discards
    }
    public override void Unregister(RelicContext ctx)
    {
        // normal dmg
    }
}

[Serializable]
public class PassiveHeal : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // heal +5 after fight
    }
    public override void Unregister(RelicContext ctx)
    {
        // no heals
    }
}

[Serializable]
public class InterestDamage : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // +damage for each interest
    }
    public override void Unregister(RelicContext ctx)
    {
        // no damage
    }
}

[Serializable]
public class Discount : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // discount
    }
    public override void Unregister(RelicContext ctx)
    {
        // normal prices
    }
}

[Serializable]
public class SlowerMinions : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // minion + turn
    }
    public override void Unregister(RelicContext ctx)
    {
        // minion normal turn
    }
}

[Serializable]
public class AssetBonus : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // asset + damage
    }
    public override void Unregister(RelicContext ctx)
    {
        // asset normal damage
    }
}

[Serializable]
public class Rampage : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // +1 damage for every 10 missing health (maybe for every 5 idk. Depends on maximum health at the startr of the run)
    }
    public override void Unregister(RelicContext ctx)
    {
        // no more damage
    }
}

[Serializable]
public class PowerBlock : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // block passive
    }
    public override void Unregister(RelicContext ctx)
    {
        // passive works
    }
}

[Serializable]
public class ConsumableCheat : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // 5 consumables = +1
    }
    public override void Unregister(RelicContext ctx)
    {
        // 5no more additional consumables
    }
}

[Serializable]
public class HigherReward : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // after fight +1 consumable
    }
    public override void Unregister(RelicContext ctx)
    {
        // no more
    }
}

[Serializable]
public class LuckyLuckyLucky : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // better rng
    }
    public override void Unregister(RelicContext ctx)
    {
        // Normal rng
    }
}

[Serializable]
public class RedGoldFever : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // +5 gold for every red card
    }
    public override void Unregister(RelicContext ctx)
    {
        // no more additional gold
    }
}

[Serializable]
public class BlackGoldFever : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // +5 gold for every black card
    }
    public override void Unregister(RelicContext ctx)
    {
        // no more additional gold
    }
}

[Serializable]
public class FasterStart : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // +2 damage for first two played cards
    }
    public override void Unregister(RelicContext ctx)
    {
        // normal dmg
    }
}

[Serializable]
public class DrawWin : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // draw --> win and 1 damage
    }
    public override void Unregister(RelicContext ctx)
    {
        // draws do not deal damage
    }
}

[Serializable]
public class SequenceBoost : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // sequences +2 damage
    }
    public override void Unregister(RelicContext ctx)
    {
    // sequences normal dmg
    }
}

