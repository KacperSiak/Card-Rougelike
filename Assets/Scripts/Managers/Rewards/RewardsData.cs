using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public struct RewardStruct
{
    public RewardsData reward;
    public int weight;
    public RewardType type;
}

public enum RewardType
{
    Positive,
    Negative,
    Neutral
}


[CreateAssetMenu(fileName = "RewardsData", menuName = "Scriptable Objects/RewardsData")]
public class RewardsData : ScriptableObject
{
    [field: SerializeField] public string rewardName { get; private set; }
    [SerializeReference] public List<Rewards> rewardEffect;
}

public class RewardContext
{
    public PlayerManager player;
    public ConsumablesAndRelicMenager consAndRelic;
    public MapManager mapManager;
    public GamblingManager gambling;

    public RewardContext()
    {
        player = PlayerManager.Instance;
        consAndRelic = ConsumablesAndRelicMenager.Instance;
        mapManager = MapManager.instance;
        gambling = GamblingManager.Instance;
    }
}

[Serializable]
public abstract class Rewards
{
    public abstract void AddReward(RewardContext ctx);
}
