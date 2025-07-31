using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

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
    public int buyPrice;
    public int sellPrice;
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
public class EqualBlackSuits : RelicEffects
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
public class EqualRedSuits : RelicEffects
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
public class GapsInSequences : RelicEffects
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
public class SingleCardDoubleDamage : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // beating a sequence with a single card deasls Double damage
    }

    public override void Unregister(RelicContext ctx)
    {
        // no more double damage
    }
}

[Serializable]
public class CleanSequenceBeatsEverything : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // Clean sequence can beat every minion
    }

    public override void Unregister(RelicContext ctx)
    {
        // no more ultrasequence
    }
}

[Serializable]
public class Discount : RelicEffects
{
    public float discount;

    public static UnityEvent<float> OnDiscountActivated = new();
    public static UnityEvent OnDiscountDeactivated = new();
    public override void Register(RelicContext ctx)
    {
        OnDiscountActivated?.Invoke(discount);
    }
    public override void Unregister(RelicContext ctx)
    {
        OnDiscountDeactivated?.Invoke();
    }
}

[Serializable]
public class FasterTravel : RelicEffects
{
    public float _speedModifier;
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
public class StartWithAdditionalDamage : RelicEffects
{
    public int damage;
    public override void Register(RelicContext ctx)
    {
        // +damage for first hit 
    }
    public override void Unregister(RelicContext ctx)
    {
        // no more additional damage at the start of the combat
    }
}

[Serializable]
public class LessHpMoreDamage : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // +1 damage for every 2 missing health
    }
    public override void Unregister(RelicContext ctx)
    {
        // no more additional damage
    }
}

[Serializable]
public class RedBonusDamage : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // plus damage for every played red card (no clue how to programm it XD)
    }
    public override void Unregister(RelicContext ctx)
    {
        // no more damage
    }
}

[Serializable]
public class BlackBonusDamage : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // plus damage for every played black card (no clue how to programm it XD)
    }
    public override void Unregister(RelicContext ctx)
    {
        // no more damage
    }
}

[Serializable]
public class PassiveHeal : RelicEffects
{
    public float healAmount;
    public override void Register(RelicContext ctx)
    {
        RoundManager.OnCombatEnd += Heal;
    }
    public override void Unregister(RelicContext ctx)
    {
        RoundManager.OnCombatEnd -= Heal;
    }

    void Heal()
    {
        PlayerOverlay.instance.currentHP += healAmount;
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
public class BonusLuck : RelicEffects
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
public class PassiveBlock : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // block passive for 2 turns
    }

    public override void Unregister(RelicContext ctx)
    {
        // no more blocks
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
public class BlockAtTheStart : RelicEffects
{
    public override void Register(RelicContext ctx)
    {
        // prevent damage for first two turns
    }

    public override void Unregister(RelicContext ctx)
    {
        // do not work XD
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

////////////////////////////////////////////////// After Demo Relics //////////////////////////////////////////

//[Serializable]
//public class Interest : RelicEffects
//{
//    public override void Register(RelicContext ctx)
//    {
//        // interest++
//    }

//    public override void Unregister(RelicContext ctx)
//    {
//        // interest come back to 5
//    }
//}

//[Serializable]
//public class HearthPlus : RelicEffects
//{
//    public override void Register(RelicContext ctx)
//    {
//        // hearths damage
//    }

//    public override void Unregister(RelicContext ctx)
//    {
//        // hearths damage no more
//    }
//}

//[Serializable]
//public class DiamnodPlus : RelicEffects
//{
//    public override void Register(RelicContext ctx)
//    {
//        // Diamonds damage
//    }

//    public override void Unregister(RelicContext ctx)
//    {
//        // diamonds damage no more
//    }
//}

//[Serializable]
//public class ClubsPlus : RelicEffects
//{
//    public override void Register(RelicContext ctx)
//    {
//        // clubs damage
//    }

//    public override void Unregister(RelicContext ctx)
//    {
//        // clubs damage no more
//    }
//}

//[Serializable]
//public class SpadesPlus : RelicEffects
//{
//    public override void Register(RelicContext ctx)
//    {
//        // spades damage
//    }

//    public override void Unregister(RelicContext ctx)
//    {
//        // spades damage no more
//    }
//}

//[Serializable]
//public class PowerStrike : RelicEffects
//{
//    public override void Register(RelicContext ctx)
//    {
//        // additional damage
//    }

//    public override void Unregister(RelicContext ctx)
//    {
//        // no more additional damage
//    }
//}

//[Serializable]
//public class UnclearDraw : RelicEffects
//{
//    public override void Register(RelicContext ctx)
//    {
//        // unclear sequence -> draw
//    }

//    public override void Unregister(RelicContext ctx)
//    {
            
//    }
//}

//[Serializable]
//public class BetterDamage : RelicEffects
//{
//    public override void Register(RelicContext ctx)
//    {
//        // block over 3 ----> +3
//    }
//    public override void Unregister(RelicContext ctx)
//    {
            
//    }
//}

//[Serializable]
//public class SameSuit : RelicEffects
//{
//    public override void Register(RelicContext ctx)
//    {
  
//        // additional damage ifg suit is the same
//    }
//    public override void Unregister(RelicContext ctx)
//    {
//        // nothing
//    }
//}

//[Serializable]
//public class AllForAll : RelicEffects
//{
//    public override void Register(RelicContext ctx)
//    {
//        // every sequence can beat every sequence
//    }

//    public override void Unregister(RelicContext ctx)
//    {
            
//    }
//}

//[Serializable]
//public class DiscardOrGold : RelicEffects
//{
//    public override void Register(RelicContext ctx)
//    {
//        // +gold for discard
//    }

//    public override void Unregister(RelicContext ctx)
//    {
//        // no more gold for discard
//    }
//}

//[Serializable]
//public class DoubleDamage : RelicEffects
//{
//    public override void Register(RelicContext ctx)
//    {
//        // double damage
//    }
//    public override void Unregister(RelicContext ctx)
//    {
//        // normal damage
//    }
//}

//[Serializable]
//public class ZeroDiscardBoost : RelicEffects
//{
//    public override void Register(RelicContext ctx)
//    {
//        // +1 damage if 0 discards
//    }
//    public override void Unregister(RelicContext ctx)
//    {
//        // normal dmg
//    }
//}

//[Serializable]
//public class InterestDamage : RelicEffects
//{
//    public override void Register(RelicContext ctx)
//    {
//        // +damage for each interest
//    }
//    public override void Unregister(RelicContext ctx)
//    {
//        // no damage
//    }
//}

//[Serializable]
//public class AssetBonus : RelicEffects
//{
//    public override void Register(RelicContext ctx)
//    {
//        // asset + damage
//    }
//    public override void Unregister(RelicContext ctx)
//    {
//        // asset normal damage
//    }
//}

//[Serializable]
//public class PowerBlock : RelicEffects
//{
//    public override void Register(RelicContext ctx)
//    {
//        // block passive
//    }
//    public override void Unregister(RelicContext ctx)
//    {
//        // passive works
//    }
//}

//[Serializable]
//public class ConsumableCheat : RelicEffects
//{
//    public override void Register(RelicContext ctx)
//    {
//        // 5 consumables = +1
//    }
//    public override void Unregister(RelicContext ctx)
//    {
//        // 5no more additional consumables
//    }
//}

//[Serializable]
//public class HigherReward : RelicEffects
//{
//    public override void Register(RelicContext ctx)
//    {
//        // after fight +1 consumable
//    }
//    public override void Unregister(RelicContext ctx)
//    {
//        // no more
//    }
//}

//[Serializable]
//public class RedGoldFever : RelicEffects
//{
//    public override void Register(RelicContext ctx)
//    {
//        // +5 gold for every red card
//    }
//    public override void Unregister(RelicContext ctx)
//    {
//        // no more additional gold
//    }
//}

//[Serializable]
//public class BlackGoldFever : RelicEffects
//{
//    public override void Register(RelicContext ctx)
//    {
//        // +5 gold for every black card
//    }
//    public override void Unregister(RelicContext ctx)
//    {
//        // no more additional gold
//    }
//}

//[Serializable]
//public class DrawWin : RelicEffects
//{
//    public override void Register(RelicContext ctx)
//    {
//        // draw --> win and 1 damage
//    }
//    public override void Unregister(RelicContext ctx)
//    {
//        // draws do not deal damage
//    }
//}

//[Serializable]
//public class SequenceBoost : RelicEffects
//{
//    public override void Register(RelicContext ctx)
//    {
//        // sequences +2 damage
//    }
//    public override void Unregister(RelicContext ctx)
//    {
//    // sequences normal dmg
//    }
//}

//[Serializable]
//public class HealingSequence : RelicEffects
//{
//    public override void Register(RelicContext ctx)
//    {
//        // healing 
//    }

//    public override void Unregister(RelicContext ctx)
//    {
//        // no more healing 
//    }
//}

