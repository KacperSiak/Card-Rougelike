using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCardManager : MonoBehaviour
{
    public static PlayerCardManager Instance { get; private set; }

    [SerializeField] private PlayerCardSpawner _spawner;
    [SerializeField] private BoxCollider _mainHand;
    [SerializeField] private BoxCollider _subMainHand;

    public List<GameObject> CardsInHand { get; private set; }
    public List<GameObject> SelectedCards { get; private set; }
    public int SelectedCardsPower { get; private set; }
    public List<CardsSOTest> _playerDeckInGame { get; private set; }

    [SerializeField] private int _cardsInHandDefault = 8;
    private int _currentCardsInHand = 0; 

    void Awake()
    {
        if (Instance == null) Instance = this;

        else Destroy(gameObject);

    }

    private void OnEnable()
    {
        RoundManager.OnCombatEnd += OnCombatEnd;
        RoundManager.OnRoundEnd += RefillCards; 
        RoundManager.OnRoundEnd += ClearSelectedStorage;
        RoundManager.OnCombatStart += StartCombat;
    }


    private void OnDisable()
    {
        RoundManager.OnCombatEnd -= OnCombatEnd;
        RoundManager.OnRoundEnd -= RefillCards;
        RoundManager.OnRoundEnd -= ClearSelectedStorage;
        RoundManager.OnCombatStart -= StartCombat;
    }

    public void StartCombat()
    {
        _playerDeckInGame = new List<CardsSOTest>(RoundManager.Instance.PlayerCards);
        SelectedCards = new List<GameObject>();
        CardsInHand = new List<GameObject>();
        SetMainHand();
        _spawner.SpawnCards(_playerDeckInGame, CardsInHand, _cardsInHandDefault);
    }

    public void SetMainHand()
    {
        if (CamMovementManager.Instance.IsFirstSet)
        {
            _spawner.MainHand = _mainHand;
            _mainHand.gameObject.SetActive(true);
            _subMainHand.gameObject.SetActive(false);
        }
        else
        {
            _spawner.MainHand = _subMainHand;
            _subMainHand.gameObject.SetActive(true);
            _mainHand.gameObject.SetActive(false);
        }
    }

    public void SetCards()
    {
        _spawner.SetCardsPosition();
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

    private void ClearSelectedStorage()
    {
        SelectedCards.Clear();
        SelectedCardsPower = 0;
    }

    public void ClearSelectedCards() 
    {
        _currentCardsInHand -= SelectedCards.Count;

        foreach (GameObject card in SelectedCards)
        { 
            CardsInHand.Remove(card);
        }

        SetCards();
        ClearSelectedStorage();
    }

    public void UpdateAfterDiscard(int amount)
    { 
        ClearSelectedCards();
        _spawner.SpawnCards(_playerDeckInGame, CardsInHand, cardsAmount: amount);
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
    
    private void RefillCards()
    {
        if(_playerDeckInGame.Count == 0) return;

        int amount = _currentCardsInHand >= _cardsInHandDefault? 1 : _cardsInHandDefault - _currentCardsInHand;

        _spawner.SpawnCards(_playerDeckInGame, CardsInHand, cardsAmount: amount);

    }

    private void OnCombatEnd()
    {
        foreach (var card in CardsInHand)
        {
            card.GetComponent<PlayerCardLogic>().ResetCard();
            card.SetActive(false);
            ObjPoolManager.Instance.ReturnCard(card);
        }
        ClearSelectedStorage();
    }

}
