using UnityEngine;
using System.Collections.Generic;

public class DeckMenager : MonoBehaviour
{
    public List<NormalCard> _deck;
    public static DeckMenager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Debug.Log("Found new Deck Menager");
    }
    public void AddCard(NormalCard normal)
    {
        _deck.Add(normal);
    }

    public void RemmoveCard(NormalCard normal)
    {
       var cardToRemove = _deck.Find(card => card.cardName == normal.cardName);

        if (cardToRemove != null)
        {
            _deck.Remove(cardToRemove);
        }
    }

    public List<NormalCard> DrawACardForShop()
    {
        List<NormalCard> _reserveDeck = new List<NormalCard>(_deck);
        List<NormalCard> _drawnCards = new List<NormalCard>();
        for (int i = 0; i<5; i++)
        {
            int index = Random.Range(0, _reserveDeck.Count);
            var normal = _reserveDeck[index];

            _drawnCards.Add(normal);
            _reserveDeck.Remove(normal);
        }
        
        return _drawnCards;
    }
}
