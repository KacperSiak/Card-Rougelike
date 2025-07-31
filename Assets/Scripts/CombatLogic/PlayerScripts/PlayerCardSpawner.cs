using System.Collections.Generic;
using UnityEngine;

public class PlayerCardSpawner : MonoBehaviour
{
    [SerializeField] private Transform _hand;
    private readonly float _minOverlap = 0f;
    private readonly float _maxOverlap = 0.25f;
    private float _cardsNumber = 0;
    private readonly float optimizer = 2.4f;

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

        }

        SetCardsPosition();

    }

   //Need to refactor and improve this part 
    public void SetCardsPosition()
    { 
        _cardsNumber = PlayerCardManager.Instance.CardsInHand.Count;
        if (_cardsNumber == 0) return;

        float handSize = _hand.gameObject.GetComponent<BoxCollider>().bounds.size.x;

        float cardSize = PlayerCardManager.Instance.CardsInHand[0].GetComponent<SpriteRenderer>().bounds.size.x;

        float overlap = Mathf.Lerp(_minOverlap, _maxOverlap, _cardsNumber / (ObjPoolManager.Instance.QueueSize + 1f));

        float spacing = cardSize - overlap;

        float totalSize = (_cardsNumber - optimizer) * spacing + cardSize;

        float emptySpace = handSize - totalSize;

        float startX = -handSize / 2f + emptySpace / 2f;


        for (int i = 0, renderPriority = 0; i < _cardsNumber; i++, renderPriority++)
        {
            GameObject card = PlayerCardManager.Instance.CardsInHand[i];

            float x = startX + i * spacing;

            card.transform.position = new Vector3(x, _hand.position.y, _hand.position.z);
            card.GetComponent<SpriteRenderer>().sortingOrder = renderPriority;

            card.GetComponent<PlayerCardLogic>().SaveProperties();
            card.SetActive(true);

        }

    }

}
