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
    [SerializeField] private List<NormalCard> _cards;
    [SerializeField] private List<BonusCard> _bonusCards;
    [SerializeField] private int _cardToDrawn;
    public Sprite icon;
    public int buyPrice;

    public void OpenPack()
    {
        if (_type == CardPackType.normal)
        {
            DrawCards(_cards);
        }
        else
        {
            DrawCards(_bonusCards);         
        }
    }

    private void DrawCards<T>(List<T> sourcelist) where T : BaseCard
    {
        var updatedList = new List<T>(sourcelist);
        var drawnCards = new List<T>();

        for (int i = 0; i<_cardToDrawn && updatedList.Count >i ; i++)
        {
            int index = Random.Range(0,updatedList.Count);
            drawnCards.Add(updatedList[index]);
            updatedList.RemoveAt(index);
        }

        foreach (var card in drawnCards)
        {
            DeckManager.instance.AddCard(card);
        }
    }
}
