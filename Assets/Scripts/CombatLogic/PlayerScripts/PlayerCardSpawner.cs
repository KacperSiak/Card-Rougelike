using System.Collections.Generic;
using UnityEngine;

public class PlayerCardSpawner : MonoBehaviour
{
    //[SerializeField] private BoxCollider _hand; // consider changing hand to this 
    [SerializeField] private Transform _hand;
    private int _cardsNumber = 0;
    private const int k_maxHandSize = 8; // consider changing this variable

    public AnimationCurve _curve;

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

		float handSize = _hand.gameObject.GetComponent<BoxCollider>().bounds.size.x;

		float cardSize = PlayerCardManager.Instance.CardsInHand[0].GetComponent<SpriteRenderer>().bounds.size.x; // change to box collider 

        float offset = cardSize - cardSize * (Mathf.Max(k_maxHandSize, _cardsNumber) - k_maxHandSize) / Mathf.Max(_cardsNumber - 1, 1);

        //Same as the above
        //float offset;
        //if (_cardsNumber <= k_maxHandSize)
        //    offset = cardSize;
        //else
        //    offset = cardSize - cardSize * (_cardsNumber - k_maxHandSize) / (float)(_cardsNumber - 1);

		float start = (_cardsNumber - 1) * -.5f * offset; // second part is the same as /2, we just do * 0.5 instead

		for (int i = 0, renderPriority = 0; i < _cardsNumber; i++, renderPriority++)
		{
			GameObject card = PlayerCardManager.Instance.CardsInHand[i];

			float x = start + i * offset;

			// Consider adding parent for pCards
			card.transform.position = new Vector3(x, _hand.position.y, _hand.position.z);
			card.GetComponent<SpriteRenderer>().sortingOrder = renderPriority;

			card.GetComponent<PlayerCardLogic>().SaveProperties();
			card.SetActive(true);

		}

    }

}
