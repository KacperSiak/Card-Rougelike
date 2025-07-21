using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCardManager : MonoBehaviour
{
    public static PlayerCardManager Instance { get; private set; }

    [SerializeField] private PlayerCardSpawner _spawner;
    public List<GameObject> CardObjects;
    public List<GameObject> SelectedCards { get; private set; }
    public int SelectedCardsPower { get; private set; }

    private List<CardsSOTest> _playerCardsInGame;
    private int _cardsInHandDefault = 6;
    private int _currentCardsInHand = 0; // if we need more than 6 cards in hand use it to build logic

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartCombat(List<CardsSOTest> playerCards)
    {
        _playerCardsInGame = new List<CardsSOTest>(playerCards);
        SelectedCards = new List<GameObject>();
        _spawner.SpawnCards(_playerCardsInGame, _cardsInHandDefault, startKey: 0);
    }

    public void AddSelectedCards(GameObject card)
    {
        SelectedCards.Add(card);
        SelectedCardsPower += card.GetComponent<PlayerCardLogic>().Card.power;
        Debug.Log("Cards added" + SelectedCards.Count);
    }

    public void RemoveSelectedCards(GameObject card)
    {
        SelectedCards.Remove(card);
        SelectedCardsPower -= card.GetComponent<PlayerCardLogic>().Card.power;
        Debug.Log("Cards removed" + SelectedCards.Count);
    }

    public void ClearSelectedCards()
    {
        _currentCardsInHand -= SelectedCards.Count;
        SelectedCards.Clear();
        SelectedCardsPower = 0;
        Debug.Log("Cards cleared" + SelectedCards.Count);
    }

    public void IncreaseCardsInHandAmount()
    {
        _currentCardsInHand++;
    }

    //for now only 6 cards in hand are generally possible, but for more need to negotiate with the team how they see that 
    public void RefillCards()
    {
        for (int i = 0; i < CardObjects.Count; i++)
        {
            if (CardObjects[i].activeSelf) continue;
            _spawner.SpawnCards(_playerCardsInGame, cardsAmount: 1, startKey: i);
        }
    }
}
