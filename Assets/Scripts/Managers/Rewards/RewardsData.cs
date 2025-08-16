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
