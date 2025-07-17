using System.Collections.Generic;
using UnityEngine;

public class PlayerCardManager : MonoBehaviour
{
    public static PlayerCardManager Instance { get; private set; }

    [SerializeField] private PlayerCardSpawner _spawner;
    public List<GameObject> CardObjects;
    public List<GameObject> SelectedCards { get; private set; }

    private List<CardsSOTest> _playerCardsInGame;
    private int _cardsInHand = 6;

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
        _spawner.SpawnCards(_playerCardsInGame, _cardsInHand);
    }

    public void AddSelectedCards(GameObject card)
    {
        SelectedCards.Add(card);
        Debug.Log("Cards added" + SelectedCards.Count);
    }

    public void RemoveSelectedCards(GameObject card)
    {
        SelectedCards.Remove(card);
        Debug.Log("Cards removed" + SelectedCards.Count);
    }

    public void ClearSelectedCards()
    {
        SelectedCards.Clear();
        Debug.Log("Cards cleared" + SelectedCards.Count);
    }

    public void RefillCards()
    {

    }
}
