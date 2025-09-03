using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChallengesMenager : PersistenSingleton<ChallengesMenager>
{
    [SerializeField] private Button _easyButton;
    [SerializeField] private Button _normalButton;
    [SerializeField] private Button _hardButton;

    [SerializeField] private List<ChallengesData> _easyChallenges;
    [SerializeField] private List<ChallengesData> _normalChallenges;
    [SerializeField] private List<ChallengesData> _hardChallenges;
    [SerializeField] private List<GameObject> _slots;

    [SerializeField] private RewardStruct _easyRewards;
    [SerializeField] private RewardStruct _normalRewards;
    [SerializeField] private RewardStruct _hardRewards;

    [SerializeField] private TextMeshProUGUI _cuurentChallText;

    private ChallengeContext _context;
    private ChallengesData _currentChallenge;

    protected override void OnSingletonAwake()
    {
        _easyButton.onClick.AddListener(DisplayEasy);
        _normalButton.onClick.AddListener(DisplayNormal);
        _hardButton.onClick.AddListener(DisplayHard);
        _context = new ChallengeContext();
    }

    protected override void OnDestroy()
    {
        _easyButton.onClick.RemoveListener(DisplayEasy);
        _normalButton.onClick.RemoveListener(DisplayNormal);
        _hardButton.onClick.RemoveListener(DisplayHard);

        base.OnDestroy();
    }

    private void DisplayEasy() => Display(_easyChallenges);
    private void DisplayNormal() => Display(_normalChallenges);
    private void DisplayHard() => Display(_hardChallenges);

    private void Display(List<ChallengesData> list)
    {
        List<ChallengesData> _temporaryList;
        _temporaryList = new List<ChallengesData>(list);
        _easyButton.gameObject.SetActive(false);
        _normalButton.gameObject.SetActive(false);
        _hardButton.gameObject.SetActive(false);
        foreach (var slot in _slots)
        {
            slot.gameObject.SetActive(true);
            int index = Random.Range(0, _temporaryList.Count);
            var slotUI = slot.GetComponent<ChallengesUITile>();
            if (slotUI != null)
            {
                slotUI.Setup(_temporaryList[index]);
                _temporaryList.Remove(_temporaryList[index]);
            }
        }
    }

    public void AddRewardForChall()
    {
        switch (_currentChallenge.difficulty)
        {
            case Difficulty.Easy:
                AddRewardByDifficulty( _easyRewards);
                break;

            case Difficulty.Normal:
                AddRewardByDifficulty(_normalRewards);
                break;

            case Difficulty.Hard:
                AddRewardByDifficulty(_hardRewards);
                break;
        }
    }

    public void AddRewardByDifficulty(RewardStruct diffRewards)
    {
        var ctx = new RewardContext();
        
        foreach (var reward in diffRewards.reward.rewardEffect)
        {
            reward.AddReward(ctx);
        }
        
        
    }

    public void ChallengeAccepted(ChallengesData challenge)
    {
        _currentChallenge = challenge;
        _cuurentChallText.text = _currentChallenge.challengeName;

        var trigger = _cuurentChallText.GetComponent<TooltipTrigger>();
        if (trigger == null) trigger = _cuurentChallText.gameObject.AddComponent<TooltipTrigger>();

        trigger.Setup(challenge.challengeDescription);
        Debug.Log("Challenge Accepted");

        foreach (var chall in _currentChallenge.challenge)
        {
            chall.Register(_context);
        }
    }

    public void ChallengeProgressUpdate(int current, int max)
    {
        _cuurentChallText.text = _currentChallenge.challengeName;
        _cuurentChallText.text += " " + current + "/" + max;
    }
}
