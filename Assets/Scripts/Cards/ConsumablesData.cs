using System;
using UnityEngine;
using System.Collections.Generic;

public enum CardRarity
{
    common,
    rare
}

public enum ConsCardType
{
    Moirae,
    OneUse
}
[CreateAssetMenu(fileName = "ConsumablesData", menuName = "Shop/Consumable")]
public class ConsumablesData : ShopItem
{
    [field: SerializeField] public string cardID { get; private set; }
    [field: SerializeField] public string cardName { get; private set; }
    [field: SerializeField, TextArea]  public string cardDescription { get; private set; }
    [field: SerializeField] public CardRarity rarity { get; private set; }
    [field: SerializeField] public ConsCardType type { get; private set; }
    [field: SerializeField] public int buyPrice { get; private set; }
    [field: SerializeField] public int sellPrice { get; private set; }


    [Header("ShaderGraph")]
    [field: SerializeField] public Texture2D atlasTexture { get; private set; }
    [field: SerializeField] public int rows {  get; private set; }
    [field: SerializeField] public int cols { get; private set; }
    [field: SerializeField] public int row { get; private set; }
    [field: SerializeField] public int col { get; private set; }


    [SerializeReference] public List<CardEffect> cardEffects;
}

public class ConsumableContext
{
    public PlayerManager player;
    public ShopMenager shop;
    public ConsumablesAndRelicMenager consAndRel;
    public DamageManager damageManager;
    public PlayerCardManager playerCardManager;
    public CombatManager combbatManager;

    public bool immune;
    public bool vulnerable;
    public bool bonusRound;
    public bool drawBlock;
    public bool selfHit;
    public ConsumableContext()
    {
        player = PlayerManager.Instance;
        shop = ShopMenager.Instance;
        consAndRel = ConsumablesAndRelicMenager.Instance;
        damageManager = DamageManager.Instance;
        playerCardManager = PlayerCardManager.Instance;
        combbatManager = CombatManager.Instance;
    }
}

[Serializable]
public abstract class CardEffect
{
    public abstract void ApplyEffect(ConsumableContext ctx);
}


//////////////////////////////////////////////////////////// Not for demo /////////////////////////////////////////


//[Serializable]
//public class RollForPack : CardEffect
//{
//    public override void ApplyEffect(ConsumableContext ctx)
//    {
//        // remove rolls
//        // gives pack
//    }
//}

//[Serializable]
//public class BetterConsumable : CardEffect
//{
//    public override void ApplyEffect(ConsumableContext ctx)
//    {
//        // destroy card get new
//    }
//}

//[Serializable]
//public class BetterCardsNoRelics : CardEffect
//{
//    public override void ApplyEffect(ConsumableContext ctx)
//    {
//        // boost hand
//        // switch off relics
//    }
//}

//[Serializable]
//public class OMGReroll : CardEffect //kinda op or useless (no idea why you want to clear whole map XD)
//{
//    public override void ApplyEffect(ConsumableContext ctx)
//    {
//        // remove deck
//        // roll new one
//    }
//}

//[Serializable]
//public class BuffRemover : CardEffect
//{
//    public override void ApplyEffect(ConsumableContext ctx)
//    {
//        //remove buff from enemy minion
//    }

//}

//[Serializable]
//public class PassiveOff : CardEffect
//{
//    public override void ApplyEffect(ConsumableContext ctx)
//    {
//        // switch off boss passive
//    }
//}

//[Serializable]
//public class QuickDraw : CardEffect
//{
//    public override void ApplyEffect(ConsumableContext ctx)
//    {
//        // draw a card;
//    }
//}

//[Serializable]
//public class OneMoreReroll : CardEffect
//{
//    public override void ApplyEffect(ConsumableContext ctx)
//    {
//        // adds reroll ticket
//    }
//}


//[Serializable]
//public class AssetSwitch : CardEffect
//{
//    public override void ApplyEffect(ConsumableContext ctx)
//    {
//        // Switch AssetCard
//    }
//}

//[Serializable]
//public class FreeTurn : CardEffect
//{
//    public override void ApplyEffect(ConsumableContext ctx)
//    {
//        // Additional turn
//    }
//}

//[Serializable]
//public class DamageAbsorb : CardEffect
//{
//    public override void ApplyEffect(ConsumableContext ctx)
//    {
//        // absord next damage
//    }
//}

//[Serializable]
//public class ItemDropBoost : CardEffect
//{
//    public override void ApplyEffect(ConsumableContext ctx)
//    {
//        // drop boost
//    }
//}

//[Serializable]
//public class BoosterPack : CardEffect
//{
//    public override void ApplyEffect(ConsumableContext ctx)
//    {
//        // get booster pack
//    }
//}

//[Serializable]
//public class DoubleGold : CardEffect
//{
//    public override void ApplyEffect(ConsumableContext ctx)
//    {
//        // double your gold
//    }
//}

//[Serializable]
//public class GhostRelic : CardEffect
//{
//    public override void ApplyEffect(ConsumableContext ctx)
//    {
//        // ghet one temporary relic
//    }
//}

//[Serializable]
//public class CallForHelp : CardEffect
//{
//    public override void ApplyEffect(ConsumableContext ctx)
//    {
//        // get 3 ranodm temporary card
//    }
//}

//[Serializable]
//public class HPBoost : CardEffect
//{
//    public override void ApplyEffect(ConsumableContext ctx)
//    {
//        // heal 10 hp
//    }
//}

//[Serializable]
//public class UltraRelic : CardEffect
//{
//    public override void ApplyEffect(ConsumableContext ctx)
//    {
//        // next relic drop boost
//    }
//}

//[Serializable]
//public class UnderDesk : CardEffect
//{
//    public override void ApplyEffect(ConsumableContext ctx)
//    {
//        // better stuff in shop
//    }
//}

//[Serializable]
//public class Mulligan : CardEffect
//{
//    public override void ApplyEffect(ConsumableContext ctx)
//    {
//        // rewshuffle hand
//    }
//}

//[Serializable]
//public class MoiraeVisit : CardEffect
//{
//    public override void ApplyEffect(ConsumableContext ctx)
//    {
//        // random morriare card
//    }
//}

//[Serializable]
//public class GodRelic : CardEffect
//{
//    public override void ApplyEffect(ConsumableContext ctx)
//    {
//        // + god relic
//    }
//}

//[Serializable]
//public class Reset : CardEffect
//{
//    public override void ApplyEffect(ConsumableContext ctx)
//    {
//        // fight reset
//    }
//}

//[Serializable]
//public class GoldForBonus : CardEffect
//{
//    public override void ApplyEffect(ConsumableContext ctx)
//    {
//        // add gold
//    }
//}

//[Serializable]
//public class CopyPaste : CardEffect
//{
//    public override void ApplyEffect(ConsumableContext ctx)
//    {
//        // copy consumapble card
//    }
//}

//[Serializable]
//public class LookIntoTheFuture : CardEffect
//{
//    public override void ApplyEffect(ConsumableContext ctx)
//    {
//        // look for cards
//    }
//}


