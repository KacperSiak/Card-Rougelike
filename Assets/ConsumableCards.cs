using UnityEngine;
using UnityEngine.Rendering;
using System;
using UnityEngine.LightTransport;

public abstract class ConsumableCards
{
    public CardVisualData cardFace;
    public ConsumableCards(CardVisualData visual)
    {
        cardFace = visual;
    }

    public abstract void ApplyEffect(PlayerOverlay player);
}

public class Sacrifice : ConsumableCards
{
    public Sacrifice(CardVisualData visual) : base(visual) {}
    public override void ApplyEffect(PlayerOverlay player)
    {
        player.currentHP *= 0.75f;
        player.addDamage = 5;
    }
}

public class RollForPack : ConsumableCards
{
    public RollForPack(CardVisualData visual): base(visual) {}
    public override void ApplyEffect(PlayerOverlay player)
    {
        // remove rolls
        // gives pack
    }
}


public class RelicBlood : ConsumableCards
{
    public RelicBlood(CardVisualData visual) : base(visual) {}
    public override void ApplyEffect(PlayerOverlay player)
    {
        player.maxHP *= 0.8f;
        // relic slots +1
    }
}

public class BetterConsumable : ConsumableCards
{
    public BetterConsumable(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // destroy card get new
    }
}

public class OnTheEdge : ConsumableCards
{
    public OnTheEdge(CardVisualData visual) : base(visual) {}

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
    public ShopBlock(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // blocks the shop
        // get two random relics
    }
}

public class TimeCheat : ConsumableCards
{
    public TimeCheat(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // immune to damage
        // next turn double damage
    }
}

public class DoubleButLess : ConsumableCards
{
    public DoubleButLess(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // you cannopt draw cards
        // double damage turn
    }
}

public class HugeReroll : ConsumableCards
{
    public HugeReroll(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // Remove all consumables
        // Add new radnom rerolls
    }
}

public class InstaWin : ConsumableCards
{
    public InstaWin(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // lose all relics
        // win fight
    }
}

public class RelicRoll : ConsumableCards
{
    public RelicRoll(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // lose relic
        // get one new after fight
    }
}

public class LegendaryReroll : ConsumableCards
{
    public LegendaryReroll(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // lose all relics
        // add 2 legendary
    }
}

public class NoMoreHeals : ConsumableCards
{
    public NoMoreHeals(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // block fountains
        // max hp +++
    }
}

public class Modifier : ConsumableCards
{
    public Modifier(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // block modifications
        // after boss more more more modifications
    }
}

public class TwoForOne : ConsumableCards
{
    public TwoForOne(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // remove 2 relics of same rarity
        // add one better rarity
    }
}

public class BetterCardsNoRelics : ConsumableCards
{
    public BetterCardsNoRelics(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // boost hand
        // switch off relics
    }
}

public class OMGReroll : ConsumableCards //kinda op or useless (no idea why you want to clear whole map XD)
{
    public OMGReroll(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // remove deck
        // roll new one
    }
}

public class InstaKill : ConsumableCards
{
    public InstaKill(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // insta kill
    }
}

public class BuffRemover : ConsumableCards
{
    public BuffRemover(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        //remove buff from enemy minion
    }

}

public class CardBuff : ConsumableCards
{
    public CardBuff(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // card damage x2/x3 (no idea if it works like 2 can be 4 in sequence after buff x2 or just damage i buffed)
    }
}

public class PassiveOff : ConsumableCards
{
    public PassiveOff(CardVisualData visual)
        : base(visual)
    { }

    public override void ApplyEffect(PlayerOverlay player)
    {
        // switch off boss passive
    }
}

public class QuickDraw : ConsumableCards
{
    public QuickDraw(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // draw a card;
    }
}

public class OneMoreReroll : ConsumableCards
{
    public OneMoreReroll(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // adds reroll ticket
    }
}

public class Jackpot : ConsumableCards
{
    public Jackpot(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // get 100 gold
    }
}

public class AssetSwitch : ConsumableCards
{
    public AssetSwitch(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // Switch AssetCard
    }
}

public class FutureSeeing : ConsumableCards
{
    public FutureSeeing(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // Look at the next enemy minion
    }
}

public class FreeTurn : ConsumableCards
{
    public FreeTurn(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // Additional turn
    }
}

public class DamageAbsorb : ConsumableCards
{
    public DamageAbsorb(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // absord next damage
    }
}

public class ItemDropBoost : ConsumableCards
{
    public ItemDropBoost(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // drop boost
    }
}

public class Retire : ConsumableCards
{
    public Retire(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // escape from fight
    }
}

public class BoosterPack : ConsumableCards
{
    public BoosterPack(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // get booster pack
    }
}

public class DoubleGold : ConsumableCards
{
    public DoubleGold(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // double your gold
    }
}

public class GhostRelic : ConsumableCards
{
    public GhostRelic(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // ghet one temporary relic
    }
}

public class CallForHelp : ConsumableCards
{
    public CallForHelp(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // get 3 ranodm temporary card
    }
}

public class CoinFlip : ConsumableCards
{
    public CoinFlip(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // 50/50 for 30 gold
    }
}

public class FreeBuy : ConsumableCards
{
    public FreeBuy(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // free buy
    }
}

public class HPBoost : ConsumableCards
{
    public HPBoost(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // heal 10 hp
    }
}

public class UltraRelic : ConsumableCards
{
    public UltraRelic(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // next relic drop boost
    }
}

public class UnderDesk : ConsumableCards
{
    public UnderDesk(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // better stuff in shop
    }
}

public class Mulligan : ConsumableCards
{
    public Mulligan(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // rewshuffle hand
    }
}

public class MoiraeVisit : ConsumableCards
{
    public MoiraeVisit(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // random morriare card
    }
}

public class HugeJackpot : ConsumableCards
{
    public HugeJackpot(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // giga op
        // get jackpot
    }
}

public class GodRelic : ConsumableCards
{
    public GodRelic(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // + god relic
    }
}

public class Reset : ConsumableCards
{
    public Reset(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // fight reset
    }
}

public class GoldForBonus : ConsumableCards
{
    public GoldForBonus(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // add gold
    }
}

public class CopyPaste : ConsumableCards
{
    public CopyPaste(CardVisualData visual) : base(visual) {}
    public override void ApplyEffect(PlayerOverlay player)
    {
        // copy consumapble card
    }
}

public class LookIntoTheFuture : ConsumableCards
{
    public LookIntoTheFuture(CardVisualData visual) : base(visual) {}

    public override void ApplyEffect(PlayerOverlay player)
    {
        // look for cards
    }
}
