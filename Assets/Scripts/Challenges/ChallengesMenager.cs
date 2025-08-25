using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengesMenager : PersistenSingleton<ChallengesMenager>
{
    [SerializeField] private Button _easyButton;
    [SerializeField] private Button _normalButton;
    [SerializeField] private Button _hardButton;

    [SerializeField] private List<ChallengesData> _easyChallenges;
    [SerializeField] private List<ChallengesData> _normalChallenges;
    [SerializeField] private List<ChallengesData> _hardChallenges;
    [SerializeField] private List<GameObject> _slots;

    [SerializeField] private CardPack _cardPack;
    [SerializeField] private VaseRelicUpgrade _vase;
    
    private ChallengesData _currentChallenge;

    protected override void OnSingletonAwake()
    {
        _easyButton.onClick.AddListener(DisplayEasy);
        _normalButton.onClick.AddListener(DisplayNormal);
        _hardButton.onClick.AddListener(DisplayHard);
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
            var slotUI = slot.GetComponent<ChallengesUI>();
            if (slotUI != null)
            {
                slotUI.Setup(_temporaryList[index]);
                _temporaryList.Remove(_temporaryList[index]);
            }
        }
    }

    public void AddReward()
    {
        int coin = Random.Range(0, 2);
        ConsumablesData cons = ConsumablesAndRelicMenager.Instance.RollRandomCons();
        if (_currentChallenge.difficulty == Difficulty.Easy)
        {
            PlayerOverlay.Instance.AddGold(25);
            _cardPack.OpePack();
        }
        else if (_currentChallenge.difficulty == Difficulty.Normal)
        {
            PlayerOverlay.Instance.AddGold(35);
            _vase.GotVase();
            if (coin == 0)
            {
                ConsumablesAndRelicMenager.Instance.AddCard(cons);
            }
        }
        else
        {
            float newHP = PlayerOverlay.Instance.maxHP + 5;
            PlayerOverlay.Instance.AddGold(45);
            PlayerOverlay.Instance.SetMaxHP(newHP);
            _vase.GotVase();
            if (coin == 0)
            {
                ConsumablesAndRelicMenager.Instance.AddCard(cons);
            }
        }
    }

    public void ChallengeAccepted(ChallengesData challenge)
    {
        _currentChallenge = challenge;
        Debug.Log("Challenge Accepted");
    }
}
