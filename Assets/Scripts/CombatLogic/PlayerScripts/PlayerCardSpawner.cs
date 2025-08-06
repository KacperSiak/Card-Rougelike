using System.Collections.Generic;
using UnityEngine;

public class PlayerCardSpawner : MonoBehaviour
{
    //[SerializeField] private BoxCollider _hand;
    [SerializeField] private Transform _hand;
    private readonly float _minOverlap = -0.01f;
    //private readonly float _minOverlap = 0f;
    private readonly float _maxOverlap = 0.1f;
    //private readonly float _maxOverlap = 0.25f;
    //private int _cardsNumber = 0;
    private float _cardsNumber = 0;
    private readonly float optimizer = 2.4f;
    private const int k_maxHandSize = 7;

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

        SetCardsPosition2();

    }

   //Need to refactor and improve this part 
    public void SetCardsPosition()
    { 
        _cardsNumber = PlayerCardManager.Instance.CardsInHand.Count;
        if (_cardsNumber == 0) return;

        float handSize = _hand.gameObject.GetComponent<BoxCollider>().bounds.size.x;

        float cardSize = PlayerCardManager.Instance.CardsInHand[0].GetComponent<SpriteRenderer>().bounds.size.x;

        float overlap = Mathf.Lerp(_minOverlap, _maxOverlap, _cardsNumber / (ObjPoolManager.Instance.QueueSize + 1f));
		//float overlap = Mathf.Lerp(_minOverlap, _maxOverlap, (float)_cardsNumber / (ObjPoolManager.Instance.QueueSize + 1f));

		float spacing = cardSize - overlap;

        float totalSize = (_cardsNumber - optimizer) * spacing + cardSize;

        float emptySpace = handSize - totalSize;

        float startX = -handSize / 2f + emptySpace / 2f;


        for (int i = 0, renderPriority = 0; i < _cardsNumber; i++, renderPriority++)
        {
            GameObject card = PlayerCardManager.Instance.CardsInHand[i];

            float x = startX + i * spacing;

            //add to parent and change locallPosition
            card.transform.position = new Vector3(x, _hand.position.y, _hand.position.z);
            card.GetComponent<SpriteRenderer>().sortingOrder = renderPriority;

            card.GetComponent<PlayerCardLogic>().SaveProperties();
            card.SetActive(true);

        }
        _curve.Evaluate(.5f);

	}


	public void SetCardsPosition2()
	{
		_cardsNumber = PlayerCardManager.Instance.CardsInHand.Count;
		if (_cardsNumber == 0) return;

		float handSize = _hand.gameObject.GetComponent<BoxCollider>().bounds.size.x;

		float cardSize = PlayerCardManager.Instance.CardsInHand[0].GetComponent<SpriteRenderer>().bounds.size.x;


        //float offset = cardSize - cardSize * (Mathf.Max(7, _cardsNumber) - 7) / (float)(Mathf.Max(_cardsNumber - 1, 1));
        //float offset = cardSize - cardSize * (_cardsNumber - 7) / (float)_cardsNumber;

        float offset;
        if (_cardsNumber <= k_maxHandSize)
            offset = cardSize;
        else
            offset = cardSize - cardSize * (_cardsNumber - k_maxHandSize) / (float)(_cardsNumber - 1);

		float start = (_cardsNumber - 1) * -.5f * offset;



		for (int i = 0, renderPriority = 0; i < _cardsNumber; i++, renderPriority++)
		{
			GameObject card = PlayerCardManager.Instance.CardsInHand[i];

			float x = start + i * offset;

			//add to parent and change locallPosition
			card.transform.position = new Vector3(x, _hand.position.y, _hand.position.z);
			card.GetComponent<SpriteRenderer>().sortingOrder = renderPriority;

			card.GetComponent<PlayerCardLogic>().SaveProperties();
			card.SetActive(true);

		}

	}

}
