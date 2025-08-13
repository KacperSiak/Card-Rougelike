using UnityEngine;
using System.Collections.Generic;

public enum CardPackType
{
    normal,
    bonus
}

[CreateAssetMenu(fileName = "CardPack", menuName = "Shop/CardPack")]
public class CardPack : ShopItem
{
    [SerializeField] private CardPackType _type;
    [SerializeField] private List<BonusCard> _cards;
    [SerializeField] private List<BonusCard> _bonusCards;
    [SerializeField] private int _cardToDrawn;
    public Sprite icon;
    public int buyPrice;

    //public void OpenPack()
    public void OpePack()
    {
        if (_type == CardPackType.normal)
        {
            var updatedList = new List<BonusCard>(_cards);
            var drawnCards = new List<BonusCard>();

            for (int i = 0; i < _cardToDrawn && updatedList.Count > i; i++)
            {
                int index = Random.Range(0, _cards.Count);
                drawnCards.Add(updatedList[index]);
                updatedList.RemoveAt(index);
            }

            foreach (var card in drawnCards)
            {
                DeckManager.instance.AddCard(card);
            }
        }
        else
        {
            var updatedList = new List<BonusCard>(_bonusCards);
            var drawnCards = new List<BonusCard>();

            for (int i = 0; i < _cardToDrawn && updatedList.Count > i; i++)
            {
                int index = Random.Range(0, _cards.Count);
                drawnCards.Add(updatedList[index]);
                updatedList.RemoveAt(index);
            }

            foreach (var card in drawnCards)
            {
                DeckManager.instance.AddCard(card);
            }
        }
    }
}
