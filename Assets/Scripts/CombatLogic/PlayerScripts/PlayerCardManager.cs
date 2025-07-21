using System.Collections.Generic;
using UnityEngine;

public class PlayerCardManager : MonoBehaviour
{
    public static PlayerCardManager Instance { get; private set; }

    [SerializeField] private PlayerCardSpawner _spawner;
    public List<GameObject> CardObjects;
    public List<GameObject> SelectedCards { get; private set; }
    public int SelectedCardsPower { get; private set; }

    private List<CardsSOTest> _playerCardsInGame;
    private int _cardsInHandStart = 6;
    private int _currentCardsInHand = 0;

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
        _spawner.SpawnCards(_playerCardsInGame, _cardsInHandStart);
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

    public void RefillCards()
    {

    }
}
