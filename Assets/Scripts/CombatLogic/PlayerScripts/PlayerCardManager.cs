using System.Collections.Generic;
using UnityEngine;

public class PlayerCardManager : MonoBehaviour
{
    public static PlayerCardManager Instance { get; private set; }

    [SerializeField] private PlayerCardSpawner _spawner;
    public List<GameObject> CardObjects;
    public List<GameObject> SelectedCards { get; private set; }
    public int SelectedCardsPower { get; private set; }

    private List<CardsSOTest> _playerDeckInGame;
    private int _cardsInHandDefault = 6;
    private int _currentCardsInHand = 0; // if we need more than 6 cards in hand use it to build logic

    void Awake()
    {
        if (Instance == null) Instance = this;

        else Destroy(gameObject);

    }

    private void OnEnable()
    {
        RoundManager.OnCombatEnd += OnCombatEnd;
        RoundManager.OnRoundEnd += RefillCards;
        RoundManager.OnRoundStart += StartCombat;
    }


    private void OnDisable()
    {
        RoundManager.OnCombatEnd -= OnCombatEnd;
        RoundManager.OnRoundEnd -= RefillCards;
        RoundManager.OnRoundStart -= StartCombat;
    }

    public void StartCombat()
    {
        _playerDeckInGame = new List<CardsSOTest>(RoundManager.Instance.PlayerCards);
        SelectedCards = new List<GameObject>();
        _spawner.SpawnCards(_playerDeckInGame, _cardsInHandDefault, startKey: 0);
    }

    public void AddSelectedCards(GameObject card)
    {
        SelectedCards.Add(card);
        SelectedCardsPower += card.GetComponent<PlayerCardLogic>().Card.power;
    }

    public void RemoveSelectedCards(GameObject card)
    {
        SelectedCards.Remove(card);
        SelectedCardsPower -= card.GetComponent<PlayerCardLogic>().Card.power;
    }

    public void ClearSelectedCards()
    {
        _currentCardsInHand -= SelectedCards.Count;
        SelectedCards.Clear();
        SelectedCardsPower = 0;
    }

    public void IncreaseCardsInHandAmount()
    {
        _currentCardsInHand++;
    }

    public bool CheckRoundReadiness()
    {
        if(_playerDeckInGame.Count == 0 && _currentCardsInHand == 0) return false;
        
        return true;
    }

    //for now only 6 cards in hand are generally possible, but for more need to negotiate with the team how they see that 
    private void RefillCards()
    {
        if(_playerDeckInGame.Count == 0) return;

        for (int i = 0; i < CardObjects.Count; i++)
        {
            if (CardObjects[i].activeSelf) continue;
            _spawner.SpawnCards(_playerDeckInGame, cardsAmount: 1, startKey: i);
        }
    }

    private void OnCombatEnd()
    {
        foreach (var card in CardObjects)
        {
            card.GetComponent<PlayerCardLogic>().ResetCard();
            card.SetActive(false);
        }
        SelectedCards.Clear();
        SelectedCardsPower = 0;
    }

}
