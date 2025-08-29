using UnityEngine;
using System.Collections.Generic;

public class DeckManager : MonoBehaviour
{
    public List<BaseCard> _deck;
    public static DeckManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Debug.Log("Found new Deck Menager");
    }
    public void AddCard(BaseCard normal)
    {
        _deck.Add(normal);
    }

    public void RemmoveCard(BaseCard normal)
    {
       var cardToRemove = _deck.Find(card => card.cardName == normal.cardName);

        if (cardToRemove != null)
        {
            _deck.Remove(cardToRemove);
        }
    }

    public List<BaseCard> DrawACardForShop()
    {
        List<BaseCard> _reserveDeck = new List<BaseCard>(_deck);
        List<BaseCard> _drawnCards = new List<BaseCard>();
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
