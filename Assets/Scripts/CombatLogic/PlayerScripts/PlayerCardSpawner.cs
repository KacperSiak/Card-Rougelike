using System.Collections.Generic;
using UnityEngine;

public class PlayerCardSpawner : MonoBehaviour
{
    public void SpawnCards(List<CardsSOTest> playerCards, int cardsAmount)
    {
        for (int i = 0; i < cardsAmount; i++)
        {
            GameObject cardObject = PlayerCardManager.Instance.CardObjects[i];
            CardsSOTest card = playerCards[Random.Range(0, playerCards.Count)];
            playerCards.Remove(card);

            cardObject.GetComponent<SpriteRenderer>().sprite = card.graphics;
            cardObject.GetComponent<PlayerCardLogic>().SetCardProperties(card);
            cardObject.SetActive(true);
        }

    }
}
