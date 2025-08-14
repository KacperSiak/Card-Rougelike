using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengesMenager : MonoBehaviour
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

    public static ChallengesMenager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else
        {
            Debug.Log("Found Another Menager");
            Destroy(gameObject);
        }

        _easyButton.onClick.AddListener(DisplayEasy);
        _normalButton.onClick.AddListener(DisplayNormal);
        _hardButton.onClick.AddListener(DisplayHard);
    }

    private void OnDestroy()
    {
        _easyButton.onClick.RemoveListener(DisplayEasy);
        _normalButton.onClick.RemoveListener(DisplayNormal);
        _hardButton.onClick.RemoveListener(DisplayHard);
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

    // MAgic numbers to move to other script (reward script)
    public void AddReward()
    {
        int coin = Random.Range(0, 2);
        ConsumablesData cons = ConsumablesAndRelicMenager.instance.RollRandomCons();
        if (_currentChallenge.difficulty == Difficulty.Easy)
        {
            PlayerOverlay.instance.AddGold(25);
            _cardPack.OpePack();
        }
        else if (_currentChallenge.difficulty == Difficulty.Normal)
        {
            PlayerOverlay.instance.AddGold(35);
            _vase.GotVase();
            if (coin == 0)
            {
                ConsumablesAndRelicMenager.instance.AddCard(cons);
            }
        }
        else
        {
            float newHP = PlayerOverlay.instance.maxHP + 5;
            PlayerOverlay.instance.AddGold(45);
            PlayerOverlay.instance.SetMaxHP(newHP);
            _vase.GotVase();
            //RelicEventsSystem.RelicAcquired(RollRelic());
            if (coin == 0)
            {
                ConsumablesAndRelicMenager.instance.AddCard(cons);
            }
        }
    }

    public void ChallengeAccepted(ChallengesData challenge)
    {
        _currentChallenge = challenge;
        Debug.Log("Challenge Accepted");
    }

    // To change into another script with rewards
    //private RelicData RollRelic()
    //{
    //    int number = Random.Range(0, 100);
    //    RelicData relic;
    //    if (number < 40) relic = ConsumablesAndRelicMenager.instance.RollCommonRelic();
    //    else if (number < 70) relic = ConsumablesAndRelicMenager.instance.RollRareRelic();
    //    else if (number < 95) relic = ConsumablesAndRelicMenager.instance.RollEpicRelic();
    //    else relic = ConsumablesAndRelicMenager.instance.RollLegendaryRelic();

    //    return relic;
    //}
}
