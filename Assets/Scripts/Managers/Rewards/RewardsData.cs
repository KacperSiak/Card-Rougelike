using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "RewardsData", menuName = "Scriptable Objects/RewardsData")]
public class RewardsData : ScriptableObject
{
    [field: SerializeField] public string rewardName { get; private set; }
    [field: SerializeField] public int dropchances { get; private set; }
    [SerializeReference] public List<Rewards> reward;
}

public class RewardContext
{
    public PlayerOverlay player;
    public ConsumablesAndRelicMenager consAndRelic;
    public MapManager mapManager;

    public RewardContext()
    {
        player = PlayerOverlay.instance;
        consAndRelic = ConsumablesAndRelicMenager.instance;
        mapManager = MapManager.instance;
    }
}

[Serializable]
public abstract class Rewards
{
    public abstract void AddReward(RewardContext ctx);
}

[Serializable]
public class AddGold : Rewards
{
    private int goldAmount;
    public override void AddReward(RewardContext ctx)
    {
        ctx.player.AddGold(goldAmount);
    }
}

[Serializable]
public class Heal : Rewards
{
    private int healAmount; 
    public override void AddReward(RewardContext ctx)
    {
        ctx.player.AddHealth(healAmount);
    }
}

[Serializable]
public class MaxHealth : Rewards
{
    private float newMaxHealth;
    public override void AddReward(RewardContext ctx)
    {
        ctx.player.SetMaxHP(newMaxHealth);
    }
}

[Serializable]
public class RandomCommonRelic : Rewards
{
    public override void AddReward(RewardContext ctx)
    {
        ctx.consAndRelic.RollCommonRelic();
    }
}

[Serializable]
public class RandomRareRelic : Rewards
{
    public override void AddReward(RewardContext ctx)
    {
        ctx.consAndRelic.RollRareRelic();
    }
}

[Serializable]
public class RandomEpicRelic : Rewards
{
    public override void AddReward(RewardContext ctx)
    {
        ctx.consAndRelic.RollEpicRelic();
    }
}

[Serializable]
public class RandomLegendaryRelic : Rewards
{
    public override void AddReward(RewardContext ctx)
    {
        ctx.consAndRelic.RollLegendaryRelic();
    }
}

[Serializable]
public class AddTime : Rewards
{
    private float time;
    public override void AddReward(RewardContext ctx)
    {
        ctx.mapManager.ModifieTime(time, true);
    }
}

[Serializable]
public class TeleportToBoss : Rewards
{
    public override void AddReward(RewardContext ctx)
    {
        // Teleport to the boss
    }
}

[Serializable]
public class LoseGold : Rewards
{
    private int gold;
    public override void AddReward(RewardContext ctx)
    {
        ctx.player.SpendGold(gold);
    }
}