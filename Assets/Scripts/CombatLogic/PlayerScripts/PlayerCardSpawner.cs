using System.Collections.Generic;
using UnityEngine;

public class PlayerCardSpawner : MonoBehaviour
{
    public void SpawnCards(List<CardsSOTest> playerCards, int cardsAmount, int startKey)
    {
        for (int i = 0, key = startKey; i < cardsAmount; i++, key++)
        {
            if(playerCards.Count == 0) return;

            GameObject cardObject = PlayerCardManager.Instance.CardObjects[key];
            CardsSOTest card = playerCards[Random.Range(0, playerCards.Count)];
            playerCards.Remove(card);

            cardObject.GetComponent<SpriteRenderer>().sprite = card.graphics;
            cardObject.GetComponent<PlayerCardLogic>().SetCardProperties(card);
            cardObject.SetActive(true);
            PlayerCardManager.Instance.IncreaseCardsInHandAmount();
        }

    }
}
