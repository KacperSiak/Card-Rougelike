using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ConsumablesData", menuName = "Scriptable Objects/ConsumablesData")]
public class ConsumablesData : ScriptableObject
{
    public string cardName;
    [TextArea] public string cardDescription;
    public Sprite icon;
    [SerializeReference] public List<CardEffect> cardEffects;
}


[Serializable]
public abstract class CardEffect
{
    public abstract void ApplyEffect(PlayerOverlay player);
}

[Serializable]
public class Sacrifice : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        //player.currentHP *= 0.75f;
        //player.addDamage = 5;
        Debug.Log("Odpalam siê!");
    }
}

[Serializable]
public class RollForPack : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // remove rolls
        // gives pack
        Debug.Log("Ja te¿ siê odpalam!");
    }
}

[Serializable]
public class RelicBlood : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        //player.maxHP *= 0.8f;
        // relic slots +1
        Debug.Log("OI ja te¿ :)!");
    }
}

[Serializable]
public class BetterConsumable : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // destroy card get new
    }
}

[Serializable]
public class OnTheEdge : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        player.maxHP = 5;
        player.currentHP = 5;
        // after 2 turn going back to previous hp
        // immune to debuffs
    }
}

[Serializable]
public class ShopBlock : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // blocks the shop
        // get two random relics
    }
}

[Serializable]
public class TimeCheat : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // immune to damage
        // next turn double damage
    }
}

[Serializable]
public class DoubleButLess : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // you cannopt draw cards
        // double damage turn
    }
}

[Serializable]
public class HugeReroll : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // Remove all consumables
        // Add new radnom rerolls
    }
}

[Serializable]
public class InstaWin : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // lose all relics
        // win fight
    }
}

[Serializable]
public class RelicRoll : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // lose relic
        // get one new after fight
    }
}

[Serializable]
public class LegendaryReroll : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // lose all relics
        // add 2 legendary
    }
}

[Serializable]
public class NoMoreHeals : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // block fountains
        // max hp +++
    }
}

[Serializable]
public class Modifier : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // block modifications
        // after boss more more more modifications
    }
}

[Serializable]
public class TwoForOne : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // remove 2 relics of same rarity
        // add one better rarity
    }
}

[Serializable]
public class BetterCardsNoRelics : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // boost hand
        // switch off relics
    }
}

[Serializable]
public class OMGReroll : CardEffect //kinda op or useless (no idea why you want to clear whole map XD)
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // remove deck
        // roll new one
    }
}

[Serializable]
public class InstaKill : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // insta kill
    }
}

[Serializable]
public class BuffRemover : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        //remove buff from enemy minion
    }

}

[Serializable]
public class CardBuff : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // card damage x2/x3 (no idea if it works like 2 can be 4 in sequence after buff x2 or just damage i buffed)
    }
}

[Serializable]
public class PassiveOff : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // switch off boss passive
    }
}

[Serializable]
public class QuickDraw : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // draw a card;
    }
}

[Serializable]
public class OneMoreReroll : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // adds reroll ticket
    }
}

[Serializable]
public class Jackpot : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // get 100 gold
    }
}

[Serializable]
public class AssetSwitch : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // Switch AssetCard
    }
}

[Serializable]
public class FreeTurn : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // Additional turn
    }
}

[Serializable]
public class DamageAbsorb : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // absord next damage
    }
}

[Serializable]
public class ItemDropBoost : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // drop boost
    }
}

[Serializable]
public class Retire : CardEffect
{ 
    public override void ApplyEffect(PlayerOverlay player)
    {
        // escape from fight
    }
}

[Serializable]
public class BoosterPack : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // get booster pack
    }
}

[Serializable]
public class DoubleGold : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // double your gold
    }
}

[Serializable]
public class GhostRelic : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // ghet one temporary relic
    }
}

[Serializable]
public class CallForHelp : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // get 3 ranodm temporary card
    }
}

[Serializable]
public class CoinFlip : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // 50/50 for 30 gold
    }
}

[Serializable]
public class FreeBuy : CardEffect
{ 
    public override void ApplyEffect(PlayerOverlay player)
    {
        // free buy
    }
}

[Serializable]
public class HPBoost : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // heal 10 hp
    }
}

[Serializable]
public class UltraRelic : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // next relic drop boost
    }
}

[Serializable]
public class UnderDesk : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // better stuff in shop
    }
}

[Serializable]
public class Mulligan : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // rewshuffle hand
    }
}

[Serializable]
public class MoiraeVisit : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // random morriare card
    }
}

[Serializable]
public class HugeJackpot : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // giga op
        // get jackpot
    }
}

[Serializable]
public class GodRelic : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // + god relic
    }
}

[Serializable]
public class Reset : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // fight reset
    }
}

[Serializable]
public class GoldForBonus : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // add gold
    }
}

[Serializable]
public class CopyPaste : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // copy consumapble card
    }
}

[Serializable]
public class LookIntoTheFuture : CardEffect
{
    public override void ApplyEffect(PlayerOverlay player)
    {
        // look for cards
    }
}
