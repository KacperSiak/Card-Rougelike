using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.UI;

public class GamblingMenager : MonoBehaviour
{
    [SerializeField] private VaseRelicUpgrade _vase;
    [SerializeField] private CardPack _pack;
    [SerializeField] private CardPack _bonusPack;
    [SerializeField] private Button _rollButton;
    private class RewardEntry
    {
        public string name;
        public int weight;
        public Action action;

        public RewardEntry(string name, int weight, Action action)
        {
            this.name = name;
            this.weight = weight;
            this.action = action;
        }
    }
    
    private List<RewardEntry> rewards;
        
    private void Awake()
    {
        rewards = new List<RewardEntry>()
        {
            new RewardEntry("Add Relic", 4, () => AddRelic()),
            new RewardEntry("Add Time", 4, () => MapManager.instance.ModifieTime(60, true)),
            new RewardEntry("Greek Vase", 4, () => _vase.GotVase()),
            new RewardEntry("Nomal Card Pack", 4, () => _pack.OpePack()),
            new RewardEntry("Bonus Card PAck", 4, () => _bonusPack.OpePack()),

            new RewardEntry("Hermes Joke", 40, () => HermesJoke()),

            new RewardEntry("Lose Health", 5, () => PlayerOverlay.instance.LoseHealth(30)),
            new RewardEntry("Boss Teleport", 5, () => AutoTeleport()),
            new RewardEntry("Time Lose", 5, () => MapManager.instance.ModifieTime(60, false)),
            new RewardEntry("Lose Money", 5, () => PlayerOverlay.instance.SpendGold(50))
        };

        _rollButton.onClick.AddListener(Roll);
    }

    private void Roll()
    {
        PlayerOverlay.instance.SpendGold(5);
        int totalWeight = rewards.Sum(r => r.weight);
        int roll = UnityEngine.Random.Range(0, totalWeight);

        int cumulative = 0;

        foreach (var rew in rewards)
        {
            cumulative += rew.weight;

            if (roll < cumulative)
            {
                rew.action.Invoke();
                return;
            }
        }

        _rollButton.onClick.RemoveAllListeners();
    }
    
    private void AddRelic()
    {
        int coin = UnityEngine.Random.Range(0, 1);
        RelicData epicRelic = ConsumablesAndRelicMenager.instance.RollEpicRelic();
        RelicData legendaryRelic = ConsumablesAndRelicMenager.instance.RollLegendaryRelic();

        if (coin ==0 )
        {
            RelicEventsSystem.RelicAcquired(epicRelic);
        }
        else
        {
             RelicEventsSystem.RelicAcquired(legendaryRelic);
        }
    }

    private void HermesJoke()
    {
        // roll random Hermes Joke (maybe list of scriptable objects?)
    }

    private void AutoTeleport()
    {
        //teleport to boss field
    }

}
