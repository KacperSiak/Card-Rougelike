using System.Runtime.CompilerServices;
using UnityEngine;

public enum Rarity
{
    Common,
    Rare,
    Epic,
    Legendary
}
public abstract class Relic
{
    public string name;
    public string description;
    public Rarity rarity;
    public RelicVisualData icon;

    public Relic(string name, string description, Rarity rarity, RelicVisualData visual)
    {
        this.name = name;
        this.description = description;
        this.rarity = rarity;
        icon = visual;
    }

    public abstract void ActiveEffect(PlayerOverlay player);
}


public class BlackisBlack : Relic
{
    public BlackisBlack(RelicVisualData visual)
        : base("BlackisBlack", "Clubs and Spades are count as the same suit", Rarity.Rare, visual)
    {}

    public override void ActiveEffect(PlayerOverlay player)
    {
        // spades = clubs
    }
}

public class RedisRed : Relic
{
    public RedisRed(RelicVisualData visual)
        : base("RedkisRed", "Hearths and Diamnonds are count as the same suit", Rarity.Rare, visual)
    { }

    public override void ActiveEffect(PlayerOverlay player)
    {
        // hearths = diamonds
    }
}

public class Gaps : Relic
{
    public Gaps(RelicVisualData visual)
        :base("Gaps", "You can create sequences with 1 rank gaps", Rarity.Common, visual)
    {}

    public override void ActiveEffect(PlayerOverlay player)
    {
        // sequences gap
    }
}

public class HealingSequence : Relic
{
    public HealingSequence(RelicVisualData visual)
        :base("HealingSequence", "After killing enemy sequence minion you heal for one hp", Rarity.Common, visual)
    {}

    public override void ActiveEffect(PlayerOverlay player)
    {
        // healing 
    }
}

public class Interest : Relic
{
    public Interest(RelicVisualData visual)
        :base("Interest", "Rise your interest up to 10", Rarity.Rare, visual)
    {}

    public override void ActiveEffect(PlayerOverlay player)
    {
        // interest++
    }
}

public class HearthPlus : Relic
{
    public HearthPlus(RelicVisualData visual)
        :base("HearthPLus", "Yuor hearth cards deal +2 damage", Rarity.Common, visual)
    {}

    public override void ActiveEffect(PlayerOverlay player)
    {
        // hearths++
    }
}

public class DeathSave : Relic
{
    public DeathSave(RelicVisualData visual)
        :base("DeathSave", "Prevents death, give 25% health and dissapear", Rarity.Legendary, visual)
    {}

    public override void ActiveEffect(PlayerOverlay player)
    {
        // death save
    }
}

public class PowerStrike : Relic
{
    public PowerStrike(RelicVisualData visual)
        :base("PowerStrike", "First hit everyfight deals additional 5 damage", Rarity.Common, visual)
    {}

    public override void ActiveEffect(PlayerOverlay player)
    {
        // additional damage
    }
}

public class FasterThenLight : Relic
{
    public FasterThenLight(RelicVisualData visual)
        :base("FasterThanLight", "Your travel cost less time", Rarity.Epic, visual)
    {}

    public override void ActiveEffect(PlayerOverlay player)
    {
        // faster travel
    }
}

public class BetterStart : Relic
{
    public BetterStart(RelicVisualData visual)
        :base("BetterStart", "You prevent all damage for first two turns", Rarity.Epic, visual)
    {}

    public override void ActiveEffect(PlayerOverlay player)
    {
        // prevent damage irst two turns
    }
}

public class UnclearDraw : Relic
{
    public UnclearDraw(RelicVisualData visual)
        :base("UnclearDraw", "After you play successful unclear sequence you draw a card", Rarity.Rare, visual)
    {}

    public override void ActiveEffect(PlayerOverlay player)
    {
        // unclear sequence -> draw
    }
}

public class BetterDamage : Relic
{
    public BetterDamage(RelicVisualData visual)
        :base("BetterDamage", "if you block single card with power over 3 you deal +3 damage", Rarity.Common, visual)
    {}

    public override void ActiveEffect(PlayerOverlay player)
    {
        // block over 3 ----> +3
    }
}

public class AddDiscard : Relic
{
    public AddDiscard(RelicVisualData visual)
        :base("AddDiscard", "+1 discard every fight", Rarity.Common, visual)
    {}

    public override void ActiveEffect(PlayerOverlay player)
    {
        // +1 discard
    }
}

public class SameSuit : Relic
{
    public SameSuit(RelicVisualData visual)
        :base("SameSuit", "If you block single card with another single card with the same suit you deal +2 damage", Rarity.Rare, visual)
    {}

    public override void ActiveEffect(PlayerOverlay player)
    {
        // additional damage
    }
}

public class AllForAll : Relic
{
    public AllForAll(RelicVisualData visual)
        :base("AllForAll", "You can beat every combination with every combination", Rarity.Legendary, visual)
    {}

    public override void ActiveEffect(PlayerOverlay player)
    {
        // every sequence can beat every sequence
    }
}

public class DiscardOrGold : Relic
{
    public DiscardOrGold(RelicVisualData visual)
        :base("DiscardOrGold", "Gain one gold for every unused discard", Rarity.Common, visual)
    {}

    public override void ActiveEffect(PlayerOverlay player)
    {
        // +gold for discard
    }
}

public class DoubleDamage : Relic
{
    public DoubleDamage(RelicVisualData visual)
        :base("DoubleDamage", "If you beat clear sequence with single card you deal double damage", Rarity.Rare, visual)
    {}

    public override void ActiveEffect(PlayerOverlay player)
    {
        // double damage
    }
}

public class Handsize : Relic
{
    public Handsize(RelicVisualData visual)
        :base("Handsize", "+1 Handsize", Rarity.Common, visual)
    {}

    public override void ActiveEffect(PlayerOverlay player)
    {
        // handsize +1
    }
}

public class ZeroDiscardBoost : Relic
{
    public ZeroDiscardBoost(RelicVisualData visual)
        :base("ZeroDiscardBoost", "If you have 0 discard you deal +1 damage", Rarity.Rare, visual)
    {}

    public override void ActiveEffect(PlayerOverlay player)
    {
        // +1 damage if 0 discards
    }
}

public class PassiveHeal : Relic
{
    public PassiveHeal(RelicVisualData visual)
        :base("PassiveHeal", "After fight you heal 2 hp", Rarity.Common, visual)
    {}

    public override void ActiveEffect(PlayerOverlay player)
    {
        // heal +5 after fight
    }
}

public class InterestDamage : Relic
{
    public InterestDamage(RelicVisualData visual)
        :base("InterestDamage", "You deal additional damage for each interest", Rarity.Epic, visual)
    {}

    public override void ActiveEffect(PlayerOverlay player)
    {
        // +damage for each interest
    }
}

public class Discount : Relic
{
    public Discount(RelicVisualData visual)
        :base("Discount", "You get lower prices in shop", Rarity.Legendary, visual)
    {}

    public override void ActiveEffect(PlayerOverlay player)
    {
        // discount
    }
}

public class SlowerMinions : Relic
{
    public SlowerMinions(RelicVisualData visual)
        : base("SlowerMinion", "Each minion is one turn slower", Rarity.Epic, visual)
    {}

    public override void ActiveEffect(PlayerOverlay player)
    {
        // minion + turn
    }
}

public class AssetBonus : Relic
{
    public AssetBonus(RelicVisualData visual)
        :base("AssetBonus", "every time you use successfully asset card you get pernamently bonus damage", Rarity.Epic, visual)
    {}

    public override void ActiveEffect(PlayerOverlay player)
    {
        // asset + damage
    }
}

public class Rampage : Relic
{
    public Rampage(RelicVisualData visual)
        :base("Rampage", "For every 10 missing health, you gain +1 damage", Rarity.Epic, visual)
    {}

    public override void ActiveEffect(PlayerOverlay player)
    {
        // +1 damage for every 10 missing health (maybe for every 5 idk. Depends on maximum health at the startr of the run)
    }
}

public class PowerBlock : Relic
{
    public PowerBlock(RelicVisualData visual)
        :base("PowerBlock", "You block gods passive for first 5 turns of the fight", Rarity.Rare, visual)
    {}

    public override void ActiveEffect(PlayerOverlay player)
    {
        // block passive
    }
}

public class ConsumableCheat : Relic
{
    public ConsumableCheat(RelicVisualData visual)
        :base("ConsumableCheat", "For every 5 consumable cards played you get one for free", Rarity.Rare, visual)
    {}

    public override void ActiveEffect(PlayerOverlay player)
    {
        // 5 consumables = +1
    }
}

public class HigherReward : Relic
{
    public HigherReward(RelicVisualData visual)
        :base("HigherReward", "After each fight you can choose one consumable card", Rarity.Epic, visual)
    {}

    public override void ActiveEffect(PlayerOverlay player)
    {
        // after fight +1 consumable
    }
}

public class LuckyLuckyLucky : Relic
{
    public LuckyLuckyLucky(RelicVisualData visual)
        :base("LuckyLuckyLucky", "You get better rng for gambling, treasures and drops", Rarity.Legendary, visual)
    {}

    public override void ActiveEffect(PlayerOverlay player)
    {
        // better rng
    }
}

public class RedGoldFever : Relic
{
    public RedGoldFever(RelicVisualData visual)
        :base("RedGoldFever", "For every red card player you get 5 gold", Rarity.Rare, visual)
    {}

    public override void ActiveEffect(PlayerOverlay player)
    {
        // +5 gold for every red card
    }
}

public class BlackGoldFever : Relic
{
    public BlackGoldFever(RelicVisualData visual)
        : base("BlackGoldFever", "For every black card player you get 5 gold", Rarity.Rare, visual)
    { }

    public override void ActiveEffect(PlayerOverlay player)
    {
        // +5 gold for every black card
    }
}

public class FasterStart : Relic
{
    public FasterStart(RelicVisualData visual)
        :base("FasterStart", "First two cards you play every fight deal +2 damage", Rarity.Common, visual)
    {}

    public override void ActiveEffect(PlayerOverlay player)
    {
        // +2 damage for first two played cards
    }
}

public class DrawWin : Relic
{
    public DrawWin(RelicVisualData visual)
        :base("DrawWin", "Yuo can win with draws dealing 1 damage", Rarity.Common, visual)
    {}

    public override void ActiveEffect(PlayerOverlay player)
    {
        // draw --> win and 1 damage
    }
}

public class SequenceBoost : Relic
{
    public SequenceBoost(RelicVisualData visual)
        :base("SeQuenceBoost", "Every sequence deal additional 2 damage", Rarity.Rare, visual)
    {}

    public override void ActiveEffect(PlayerOverlay player)
    {
        // sequences +2 damage
    }
}
