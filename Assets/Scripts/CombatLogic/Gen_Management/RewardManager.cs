using UnityEngine;

public class RewardManager : PersistenSingleton<RewardManager>
{
    [SerializeField] private RewardPanelLogic _rewardPanelLogic;

    public void HandleReward()
    {
        _rewardPanelLogic.ShowReward(); 
    }
}
