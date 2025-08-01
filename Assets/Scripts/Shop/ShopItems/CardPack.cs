using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "CardPack", menuName = "Shop/CardPack")]
public class CardPack : ShopItem
{
    [SerializeField] private List<NormalCard> _cards;
    [SerializeField] private int _cardToDrawn;
    public Sprite icon;
    public int buyPrice;

    public void OpePack()
    {
        var updatedList = new List<NormalCard>(_cards);
        var drawnCards = new List<NormalCard>();

        for (int i=0; i<_cardToDrawn && updatedList.Count > i; i++)
        {
            int index = Random.Range(0, _cards.Count);
            drawnCards.Add(updatedList[index]);
            updatedList.RemoveAt(index);
        }

        foreach (var card in drawnCards)
        {
            DeckMenager.instance.AddCard(card);
        }
    }
}
