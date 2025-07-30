using System.Collections.Generic;
using UnityEngine;

public class PlayerCardSpawner : MonoBehaviour
{
    [SerializeField] private Transform _hand;
    private float _minOverlap = 0.1f;
    private float _maxOverlap = 0.4f;
    private int _cardsNumber = 0;

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

    public void SetCardsPosition()
    { 
        _cardsNumber = PlayerCardManager.Instance.CardsInHand.Count;
        if (_cardsNumber == 0) return;

        float handSize = _hand.GetComponent<MeshRenderer>().bounds.size.x;

        Vector3 leftEdge = _hand.position - _hand.right * (handSize / 2f);

        float cardSize = PlayerCardManager.Instance.CardsInHand[0].GetComponent<SpriteRenderer>().bounds.size.x;

        float indexOverlap = Mathf.Lerp(0, 1, _cardsNumber / ObjPoolManager.Instance.QueueSize);

        float overlap = Mathf.Lerp(_minOverlap, _maxOverlap, indexOverlap);

        float spacing = cardSize - overlap;

        float totalSize = (_cardsNumber - 2.5f) * spacing + cardSize;

        float emptySpace = handSize - totalSize;

        Vector3 startPos = leftEdge + _hand.right * (emptySpace / 2f);

        float startX = -spacing * (_cardsNumber - 1) / 2f;

        for (int i = 0; i < _cardsNumber; i++)
        {
            GameObject card = PlayerCardManager.Instance.CardsInHand[i];

            //card.transform.SetParent(_hand);
            card.transform.position = startPos + _hand.right * (i * spacing);
            card.transform.position += new Vector3(0f, 0f, 0.01f);

            card.GetComponent<PlayerCardLogic>().SaveProperties();
            card.SetActive(true);
            //card.transform.SetSiblingIndex(i);
        }

    }

}
