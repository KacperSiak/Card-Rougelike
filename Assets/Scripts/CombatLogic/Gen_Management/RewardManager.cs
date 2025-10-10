using MapSystem;
using System.Collections.Generic;
using UnityEngine;

public class RewardManager : PersistenSingleton<RewardManager>
{
    [SerializeField] private RewardPanelLogic _rewardPanelLogic;
    [SerializeField] private Sprite _goldSprite;
    [SerializeField] private List<StructForDictionary<NodeType, int>> _gold;
    private Dictionary<NodeType, int> _goldRewardDetails = new Dictionary<NodeType, int>();
    private int _maxGoldIndex = 16;

    private void Start()
    {
        foreach (var item in _gold)
        {
            _goldRewardDetails.Add(item.key, item.value);  
        }
    }

    public void HandleReward()
    {
        CountGold();
        _rewardPanelLogic.ShowRewards(); 
    }

    private void CountGold()
    {
        int minGold = _goldRewardDetails[CombatManager.Instance.CurrentNodeType]; 
        int gold = Random.Range(minGold, minGold + _maxGoldIndex);
        PlayerManager.Instance.GoldChange(gold, true);
        _rewardPanelLogic.SetReward(_goldSprite, $"+ {gold} Drachma");
    }
}
