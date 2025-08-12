using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class GamblingManager : MonoBehaviour
{
    [SerializeField] private VaseRelicUpgrade _vase;
    [SerializeField] private CardPack _pack;
    [SerializeField] private CardPack _bonusPack;
    [SerializeField] private Button _rollButton;
    [SerializeField] private int _positiveWeight = 4;
    [SerializeField] private int _negativeWeight = 5;
    [SerializeField] private int _neutralWeight = 40;
    [SerializeField] private List<HermesJoke> _hermesJokeList;
    [SerializeField] private TextMeshProUGUI _jokeText;
    [SerializeField] private Image _jokeSprite;

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
        _positiveWeight = (int)Math.Round(modifier * _positiveWeight);
        _jokeText.gameObject.SetActive(false);
        _jokeSprite.gameObject.SetActive(false);
        rewards = new List<RewardEntry>()
        {
            new RewardEntry("Add Relic", _positiveWeight, () => AddRelic()),
            new RewardEntry("Add Time", _positiveWeight, () => MapManager.instance.ModifieTime(60, true)),
            new RewardEntry("Greek Vase", _positiveWeight, () => _vase.GotVase()),
            new RewardEntry("Nomal Card Pack", _positiveWeight, () => _pack.OpePack()),
            new RewardEntry("Bonus Card PAck", _positiveWeight, () => _bonusPack.OpePack()),

            new RewardEntry("Hermes Joke", _neutralWeight, () => HermesJoke()),

            new RewardEntry("Lose Health", _negativeWeight, () => PlayerOverlay.instance.LoseHealth(30)),
            new RewardEntry("Boss Teleport", _negativeWeight, () => AutoTeleport()),
            new RewardEntry("Time Lose", _negativeWeight, () => MapManager.instance.ModifieTime(60, false)),
            new RewardEntry("Lose Money", _negativeWeight, () => PlayerOverlay.instance.SpendGold(50))
        };

        _rollButton.onClick.AddListener(Roll);
    }

    public void Roll()
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
        int index = UnityEngine.Random.Range(0, _hermesJokeList.Count());
        if (_hermesJokeList[index].jokeIcon == null)
        {
            _jokeText.gameObject.SetActive(true);
            _jokeText.text = _hermesJokeList[index].joke;
        }
        else
        {
            _jokeSprite.gameObject.SetActive(true);
            _jokeSprite.sprite = _hermesJokeList[index].jokeIcon;
        }
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
