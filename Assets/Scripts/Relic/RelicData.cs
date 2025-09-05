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
    [field: SerializeField] public string relicID { get; private set; } 
    [field: SerializeField] public string relicName { get; private set; }
    [field: SerializeField, TextArea] public string description;
    [field: SerializeField] public Sprite icon { get; private set; }
    [field: SerializeField] public Rarity rarity { get; private set; }
    [field: SerializeField] public int buyPrice { get; private set; }
    [field: SerializeField] public int sellPrice { get; private set; }
    [field: SerializeField] public bool Upgradeable { get; private set; }
    [SerializeReference] public List<RelicEffects> relicEffets;
}

public class RelicContext
{
    public PlayerManager player;
    public MapManager mapManager;
    public RoundManager roundManager;
    public GamblingManager gambler;


    public bool CleanSequenceBeatEverythingEnable { get; set; }
    public RelicContext()
    {
        player = PlayerManager.Instance;
        mapManager = MapManager.instance;
        roundManager = RoundManager.Instance;
        gambler = GamblingManager.Instance;
    }
}

[Serializable]
public abstract class RelicEffects
{
    public abstract void Register(RelicContext ctx);
    public abstract void Unregister(RelicContext ctx);
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

