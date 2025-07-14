using UnityEngine;
using UnityEngine.Rendering;
using System;
using UnityEngine.LightTransport;

public abstract class ConsumableCards
{
    public string name;
    public string description;
    public CardVisualData cardFace;

    public ConsumableCards(string name, string description, CardVisualData visual)
    {
        this.name = name;
        this.description = description;
        cardFace = visual;
    }

    public abstract void ApplyEffect(PlayerOverlay player);
}

public class Sacrifice : ConsumableCards
{
    public Sacrifice(CardVisualData visual)
        : base("Sacrifice", "You lose 25% of your hp but next turn you deal 5 more dmg for every killed minion", visual)
    {}
    public override void ApplyEffect(PlayerOverlay player)
    {
        player.currentHP *= 0.75f;
        player.addDamage = 5;
    }
}

public class RollForPack : ConsumableCards
{
    public RollForPack(CardVisualData visual)
        :base ("RollForPack", "You lose all roll tickets for shops but you get the pack of cards", visual)
    {}
    public override void ApplyEffect(PlayerOverlay player)
    {
        // remove rolls
        // gives pack
    }
}

public class RelicBlood: ConsumableCards
{
    public RelicBlood(CardVisualData visual)
        : base("RelicBlood", "You lose 20% o your Health but you get relic slot", visual)
    {}
    public override void ApplyEffect(PlayerOverlay player)
    {
        player.maxHP *= 0.8f;
        // relic slots +1
    }
}

public class BetterConsumable : ConsumableCards
{
    public BetterConsumable(CardVisualData visual)
        : base("BetterConsumable", "You lose chosen consumable card but you get another with the same rarity", visual)
    { }

    public override void ApplyEffect(PlayerOverlay player)
    {
        // destroy card get new
    }
}

public class OnTheEdge : ConsumableCards
{
    public OnTheEdge(CardVisualData visual)
        : base("OnTheEdge", "Yuor max hp lower to 5 but you are immune to debuffs. Lasts 2 turns", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        player.maxHP = 5;
        player.currentHP = 5;
        // after 2 turn going back to previous hp
        // immune to debuffs
    }
}

public class ShopBlock : ConsumableCards
{
    public ShopBlock(CardVisualData visual)
        : base("ShopBlock", "You cannot Visit shops unitl end of the game but you get 2 random relics", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // blocks the shop
        // get two random relics
    }
}

public class TimeCheat : ConsumableCards
{
    public TimeCheat(CardVisualData visual)
        : base("TimeCheat", "You get double damage next turn but you are immune to damage during this turn", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // immune to damage
        // next turn double damage
    }
}

public class DoubleButLess : ConsumableCards
{
    public DoubleButLess(CardVisualData visual)
        : base("DoubleButLess", "You cannot draw cards until your next turn but you deal double dmage", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // you cannopt draw cards
        // double damage turn
    }
}

public class HugeReroll : ConsumableCards
{
    public HugeReroll(CardVisualData visual)
        :base ("HugeReroll","You lose your all Moiirae cards and consumables but you get new on their place", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // Remove all consumables
        // Add new radnom rerolls
    }
}

public class InstaWin : ConsumableCards
{
    public InstaWin(CardVisualData visual)
        :base("InstaWin", "You lose all relics but you win fight (boss prevented)", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // lose all relics
        // win fight
    }
}

public class RelicRoll : ConsumableCards
{
    public RelicRoll(CardVisualData visual)
        :base("RelicRoll", "You lose one relic. After fight you can choose one from better rarity", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // lose relic
        // get one new after fight
    }
}

public class LegendaryReroll : ConsumableCards
{
    public LegendaryReroll(CardVisualData visual)
        : base("LegendaryReroll", "You lose all relics and get two random legendary relics", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // lose all relics
        // add 2 legendary
    }
}

public class NoMoreHeals : ConsumableCards
{
    public NoMoreHeals(CardVisualData visual)
        : base("NoMoreHeals", "You can't heal from fountians but you get 50% of your max hp", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // block fountains
        // max hp +++
    }
}

public class Modifier : ConsumableCards
{
    public Modifier(CardVisualData visual)
        : base("Modifier", "You cannot modify your deck until end of the act but after boss you can remove and add up to 10 cards", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // block modifications
        // after boss more more more modifications
    }
}

public class TwoForOne : ConsumableCards
{
    public TwoForOne(CardVisualData visual)
        :base("RelicRol", "Remove two random relic of the same rarity but adds one with better rarity", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // remove 2 relics of same rarity
        // add one better rarity
    }
}

public class BetterCardsNoRelics : ConsumableCards
{
    public BetterCardsNoRelics(CardVisualData visual)
        : base("BetterCardNoRelics", "Your whole hands get buff +2 but swtich off relics til the end of the fight", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // boost hand
        // switch off relics
    }
}

public class OMGReroll : ConsumableCards //kinda op or useless (no idea why you want to clear whole map XD)
{
    public OMGReroll(CardVisualData visual)
        : base("OMGReroll", "Reroll your whole deck and relics but you clear whole map", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // remove deck
        // roll new one
    }
}

public class InstaKill : ConsumableCards
{
    public InstaKill(CardVisualData visual)
        :base("InstaKill", "Destoy minion", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // insta kill
    }
}

public class BuffRemover : ConsumableCards
{
    public BuffRemover(CardVisualData visual)
        :base("BuffRemover", "Remove buffs from enemy minion", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        //remove buff from enemy minion
    }

}

public class CardBuff : ConsumableCards
{
    public CardBuff(CardVisualData visual)
        :base("CardBuff", "Card counts X2/X3", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // card damage x2/x3 (no idea if it works like 2 can be 4 in sequence after buff x2 or just damage i buffed)
    }
}

public class PassiveOff : ConsumableCards
{
    public PassiveOff(CardVisualData visual)
        : base("PassiveOff", "Switch off boss passive for one turn", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // switch off boss passive
    }
}

public class QuickDraw : ConsumableCards
{
    public QuickDraw(CardVisualData visual)
        :base("QuickDraw", "Draw a card", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // draw a card;
    }
}

public class OneMoreReroll : ConsumableCards
{
    public OneMoreReroll(CardVisualData visual)
        : base("OneMoreReroll", "Add one shop reroll", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // adds reroll ticket
    }
}

public class Jackpot : ConsumableCards
{
    public Jackpot(CardVisualData visual)
        :base("Jackpot", "Give 100 gold", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // get 100 gold
    }
}

public class AssetSwitch : ConsumableCards
{
    public AssetSwitch(CardVisualData visual)
        :base("AssetSwitch", "Switch AssetCard", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // Switch AssetCard
    }
}

public class FutureSeeing : ConsumableCards
{
    public FutureSeeing(CardVisualData visual)
        : base("FutureSeeing", "You can check next enemy minion", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // Look at the next enemy minion
    }
}

public class FreeTurn : ConsumableCards
{
    public FreeTurn(CardVisualData visual)
        : base("FreeTurn", "You get one free additional turn", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // Additional turn
    }
}

public class DamageAbsorb : ConsumableCards
{
    public DamageAbsorb(CardVisualData visual)
        : base("DamageAbsord", "Absorb next damage you would get", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // absord next damage
    }
}

public class ItemDropBoost : ConsumableCards
{
    public ItemDropBoost(CardVisualData visual)
        : base("ItemDropBoost", "Your next item has higher chances for better rarity", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // drop boost
    }
}

public class Retire : ConsumableCards
{
    public Retire(CardVisualData visual)
        :base("Retire", "You can escape from fight", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // escape from fight
    }
}

public class BoosterPack : ConsumableCards
{
    public BoosterPack(CardVisualData visual)
        :base("BoosterPack", "You get booster pack", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // get booster pack
    }
}

public class DoubleGold : ConsumableCards
{
    public DoubleGold(CardVisualData visual)
        :base("DoubleGold", "Double your gold (max 30)", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // double your gold
    }
}

public class GhostRelic : ConsumableCards
{
    public GhostRelic(CardVisualData visual)
        : base("GhostRelic", "You get one relic only for next fight even if your all slots are taken", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // ghet one temporary relic
    }
}

public class CallForHelp : ConsumableCards
{
    public CallForHelp(CardVisualData visual)
        : base("CallForHelp", "Get 3 random temporary normal card", visual)
    { }

    public override void ApplyEffect(PlayerOverlay player)
    {
        // get 3 ranodm temporary card
    }
}

public class CoinFlip : ConsumableCards
{
    public CoinFlip(CardVisualData visual)
        :base("CoinFlip", "You have 50% to get 30 gold", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // 50/50 for 30 gold
    }
}

public class FreeBuy : ConsumableCards
{
    public FreeBuy(CardVisualData visual)
        :base("FreeBuy", "Your next buy in shop is free", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // free buy
    }
}

public class HPBoost : ConsumableCards
{
    public HPBoost(CardVisualData visual)
        :base("HPBoost", "Get 10 health", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // heal 10 hp
    }
}

public class UltraRelic : ConsumableCards
{
    public UltraRelic(CardVisualData visual)
        :base("UltraRelic", "Next relic is epic or legendary", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // next relic drop boost
    }
}

public class UnderDesk : ConsumableCards
{
    public UnderDesk(CardVisualData visual)
        :base("UnderDesk", "(you can use it only in shop) Merchant show you his hidden stuff", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // better stuff in shop
    }
}

public class Mulligan : ConsumableCards
{
    public Mulligan(CardVisualData visual)
        :base("Mulligan", "Shuffle your hand int your deck and draw that many cards", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // rewshuffle hand
    }
}

public class MoiraeVisit : ConsumableCards
{
    public MoiraeVisit(CardVisualData visual)
        :base("MoiraeVisit", "Get random Moira card", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // random morriare card
    }
}

public class HugeJackpot : ConsumableCards
{
    public HugeJackpot(CardVisualData visual)
        :base("HugeJackpot", "Get one relic, one moirae card, one pack and 10 gold", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // giga op
        // get jackpot
    }
}

public class GodRelic : ConsumableCards
{
    public GodRelic(CardVisualData visual)
        :base("GodRelic", "Get GodRelic for next boss fight (but noit the relic of next boss)", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // + god relic
    }
}

public class Reset : ConsumableCards
{
    public Reset(CardVisualData visual)
        :base("Reset", "Reset whole fight", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // fight reset
    }
}

public class GoldForBonus : ConsumableCards
{
    public GoldForBonus(CardVisualData visual)
        :base("GoldForBonus", "Get 2 gold for every card above 52 cards in your deck", visual)
    {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // add gold
    }
}
