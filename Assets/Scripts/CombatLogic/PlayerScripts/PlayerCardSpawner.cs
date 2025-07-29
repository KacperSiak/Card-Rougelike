using System.Collections.Generic;
using UnityEngine;

public class PlayerCardSpawner : MonoBehaviour
{
    //Prevent any spawn if there is no more cards available in all places, work will returning to the pool and positions 
    [SerializeField] private Transform _hand;
    private float _maxAngle = 15f;
    private float _overlap = 0.2f;

    public void SpawnCards(List<CardsSOTest> playerDeck, List<GameObject> playerCards,  int cardsAmount)
    {
        for (int i = 0; i < cardsAmount; i++)
        {
            if(playerDeck.Count == 0) break;

            GameObject cardObject = ObjPoolManager.Instance.GetCardFromPool();
            playerCards.Add(cardObject);
            PlayerCardManager.Instance.IncreaseCardsInHandAmount();

            CardsSOTest card = playerDeck[Random.Range(0, playerDeck.Count)];
            playerDeck.Remove(card);

            cardObject.GetComponent<SpriteRenderer>().sprite = card.graphics;
            cardObject.GetComponent<PlayerCardLogic>().SetCardProperties(card);
            //cardObject.SetActive(true); Later and need to add card to cards in hand and after all call set position to arrange cards in a right way
        }

        SetCardsPosition();

    }

    //Always when i use and spawn cards
    public void SetCardsPosition()
    { 
    
    }

}
