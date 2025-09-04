using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCardManager : PersistenSingleton<PlayerCardManager>
{
    [SerializeField] private PlayerCardSpawner _spawner;
    [SerializeField] private BoxCollider _mainHand;
    [SerializeField] private BoxCollider _subMainHand;

    public static event Action OnViewChanged;

    public List<GameObject> CardsInHand { get; private set; } = new();
    public List<GameObject> SelectedCards { get; private set; } = new();
    public int SelectedCardsPower { get; private set; }
    public List<CardsSOTest> PlayerDeckInGame { get; private set; }

    [SerializeField] private int _cardsInHandDefault = 8;
    private int _currentCardsInHand = 0; 

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
        PlayerDeckInGame = new List<CardsSOTest>(RoundManager.Instance.PlayerCards);
        SelectedCards.Clear();
        CardsInHand.Clear();
        SetMainHand();
        _spawner.SpawnCards(PlayerDeckInGame, CardsInHand, _cardsInHandDefault);
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

        OnViewChanged?.Invoke();
    }

    public GameObject GetMainHand()
    {
        return CamMovementManager.Instance.IsFirstSet? _mainHand.gameObject : _subMainHand.gameObject;
    }

    public void SetCards()
    {
        _spawner.StartPlacement();
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
        _spawner.SpawnCards(PlayerDeckInGame, CardsInHand, cardsAmount: amount);
    }

    public void IncreaseCardsInHandAmount()
    {
        _currentCardsInHand++;
    }

    public bool CheckRoundReadiness()
    {
        if(PlayerDeckInGame.Count == 0 && _currentCardsInHand == 0) return false;
        
        return true;
    }
    
    private void RefillCards()
    {
        if (PlayerDeckInGame.Count == 0) return;

        int amount = _currentCardsInHand >= _cardsInHandDefault? 1 : _cardsInHandDefault - _currentCardsInHand;

        _spawner.SpawnCards(PlayerDeckInGame, CardsInHand, cardsAmount: amount);

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
