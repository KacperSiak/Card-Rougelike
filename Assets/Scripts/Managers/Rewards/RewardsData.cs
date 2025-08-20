using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public struct RewardStruct
{
    public RewardsData reward;
    public int weight;
}



[CreateAssetMenu(fileName = "RewardsData", menuName = "Scriptable Objects/RewardsData")]
public class RewardsData : ScriptableObject
{
    [field: SerializeField] public string rewardName { get; private set; }
    [SerializeReference] public List<Rewards> rewardEffect;
}

public class RewardContext
{
    public PlayerOverlay player;
    public ConsumablesAndRelicMenager consAndRelic;
    public MapManager mapManager;
    public GamblingManager gambling;

    public RewardContext()
    {
        player = PlayerOverlay.instance;
        consAndRelic = ConsumablesAndRelicMenager.instance;
        mapManager = MapManager.instance;
        gambling = GamblingManager.instance;
    }
}

[Serializable]
public abstract class Rewards
{
    public abstract void AddReward(RewardContext ctx);
}
