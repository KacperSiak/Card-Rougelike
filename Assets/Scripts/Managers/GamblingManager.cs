using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Events;

public class GamblingManager : MonoBehaviour
{
    [SerializeField] private VaseRelicUpgrade _vase;
    [SerializeField] private CardPack _pack;
    [SerializeField] private CardPack _bonusPack;
    [SerializeField] private Button _rollButton;
    [SerializeField] private int positiveWeight = 4;
    [SerializeField] private int negativeWeight = 5;
    [SerializeField] private int neutralWeight = 40;

    public static GamblingManager instance;

    public static UnityEvent OnGamblingExit = new UnityEvent();

    private float modifier;
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
        if (instance == null)
        {
            instance = this;
        }
        else Debug.Log("Found Another GamblingMenager");
        positiveWeight = (int)Math.Round(modifier * positiveWeight);
        rewards = new List<RewardEntry>()
        {
            new RewardEntry("Add Relic", positiveWeight, () => AddRelic()),
            new RewardEntry("Add Time", positiveWeight, () => MapManager.instance.ModifieTime(60, true)),
            new RewardEntry("Greek Vase", positiveWeight, () => _vase.GotVase()),
            new RewardEntry("Nomal Card Pack", positiveWeight, () => _pack.OpePack()),
            new RewardEntry("Bonus Card PAck", positiveWeight, () => _bonusPack.OpePack()),

            new RewardEntry("Hermes Joke", neutralWeight, () => HermesJoke()),

            new RewardEntry("Lose Health", negativeWeight, () => PlayerOverlay.instance.LoseHealth(30)),
            new RewardEntry("Boss Teleport", negativeWeight, () => AutoTeleport()),
            new RewardEntry("Time Lose", negativeWeight, () => MapManager.instance.ModifieTime(60, false)),
            new RewardEntry("Lose Money", negativeWeight, () => PlayerOverlay.instance.SpendGold(50))
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

        if (coin ==0)
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

    public void SetModifier(float newModifier)
    {
        modifier = newModifier;
    }

}
