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


