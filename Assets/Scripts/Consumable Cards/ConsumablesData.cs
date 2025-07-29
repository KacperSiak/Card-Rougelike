using System;
using UnityEngine;
using System.Collections.Generic;
using System.Globalization;

public enum CardRarity
{
    common,
    rare
}
[CreateAssetMenu(fileName = "ConsumablesData", menuName = "Shop/Consumable")]
public class ConsumablesData : ShopItem
{
    public string cardID;
    public string cardName;
    [TextArea] public string cardDescription;
    public Sprite icon;
    public CardRarity rarity;
    public int sellPrice;
    [SerializeReference] public List<CardEffect> cardEffects;
}

public class ConsumableContext
{
    public PlayerOverlay player;
    public ShopMenager shop;

    public ConsumableContext()
    {
        player = PlayerOverlay.instance;
        shop = ShopMenager.instance;
    }
}


[Serializable]
public abstract class CardEffect
{
    public abstract void ApplyEffect(ConsumableContext ctx);
}

[Serializable]
public class Draw3 : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        // draw 3 cards
    }
}

[Serializable]
public class ExtraTurn : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        // extra turn
    }
}

[Serializable]
public class FutureSeeing : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
       // look at he next minion of your oponent
    }
}

[Serializable]
public class HandReshuffle : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        // Reshuffle your hand
    }
}

[Serializable]
public class InstaHeal : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        // heal for 50 hp
    }
}

[Serializable]
public class InstaKill : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        // insta kill
    }
}

[Serializable]
public class Escape : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        // escape from fight
    }
}

[Serializable]
public class CardBuff : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        // card damage x2/x3 (no idea if it works like 2 can be 4 in sequence after buff x2 or just damage i buffed)
    }
}

[Serializable]
public class FreeBuy : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        ctx.shop.ActiveFreBuy();
    }
}

[Serializable]
public class GoldFever : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        ctx.player.gold += 100;
    }
}

[Serializable]
public class HugeJackpot : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        // giga op
        // +1 random relic, 50 coins, a regular card pack, one moirae card
    }
}

[Serializable]
public class CoinFlip : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        int coin = UnityEngine.Random.Range(0, 2);
        if (coin == 0)
        {
            ctx.player.gold += 30;
        }
    }
}

[Serializable]
public class CopyPaste : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        // copy chosen.random one use card
    }
}

[Serializable]
public class DoubleDMGButLessDraw : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        // you cannopt draw cards
        // double damage turn
    }
}

[Serializable]
public class LoseHpGetDamage : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        //player.currentHP *= 0.75f;
        //player.addDamage = 5;
    }
}

[Serializable]
public class NoMoreHealsMoreHP: CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        // block fountains
        // max hp +++
    }
}

[Serializable]
public class DeckForMap: CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        // clear whole map
        // lose your deck and get the new random one
    }
}

[Serializable]
public class OnTheEdge : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        ctx.player.maxHP = 5;
        ctx.player.currentHP = 5;
        // after 2 turn going back to previous hp
        // immune to debuffs
    }
}

[Serializable]
public class InstaWin : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        // lose all relics
        // win fight (boss prevented)
    }
}

[Serializable]
public class OneTurnImuneNextDoubleDMG : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        // immune to damage
        // next turn double damage
    }
}

[Serializable]
public class DoubleDamageSelfHit : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        // you deal double damage next/this fight
        // you get 10% damage of your attacks
    }
}

[Serializable]
public class LowerHpRelicSlot : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        // player.maxHP *= 0.8f;
        // relic slots +1
    }
}

[Serializable]
public class TwoForOne : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        // remove 2 relics of same rarity
        // add one better rarity
    }
}

[Serializable]
public class Modifier : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        // block modifications
        // after boss more more more modifications
    }
}

[Serializable]
public class RelicRoll : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        // lose relic
        // get one new after fight
    }
}

[Serializable]
public class ShopBlock : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        // blocks the shop
        // get two random relics
    }
}

[Serializable]
public class LegendaryReroll : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        // lose all relics
        // add 2 legendary
    }
}

[Serializable]
public class HealForCoins : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        // lose all drahmas
        // heal one hp for every drahma spend this way
    }
}

[Serializable]
public class HugeReroll : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        // Remove all consumables
        // Get random new ones
    }
}

[Serializable]
public class FairCardTrade : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        // lose one card
        // get new one of the same rarity
    }
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


