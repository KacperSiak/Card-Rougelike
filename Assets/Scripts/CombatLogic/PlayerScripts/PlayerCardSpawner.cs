using System.Collections.Generic;
using UnityEngine;

public class PlayerCardSpawner : MonoBehaviour
{
    [SerializeField] private BoxCollider _hand;
    private int _cardsNumber = 0;
    private const int k_maxHandSize = 8; // max without overlay
    private float _cardSize = 0.06f;

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

            cardObject.GetComponent<PlayerCardLogic>().SetCardProperties(card);
            CardSetupManager.Instance.SetupCardVisual(cardObject.GetComponent<MeshRenderer>(), card.column, card.row);
        }

        SetCardsPosition();

    }

	public void SetCardsPosition()
	{
		_cardsNumber = PlayerCardManager.Instance.CardsInHand.Count;
		if (_cardsNumber == 0) return;

		float handSize = _hand.bounds.size.x;

        float offset = _cardSize - _cardSize * (Mathf.Max(k_maxHandSize, _cardsNumber) - k_maxHandSize) / Mathf.Max(_cardsNumber - 1, 1);

        //Same as the above
        //float offset;
        //if (_cardsNumber <= k_maxHandSize)
        //    offset = cardSize;
        //else
        //    offset = cardSize - cardSize * (_cardsNumber - k_maxHandSize) / (float)(_cardsNumber - 1);

		float start = (_cardsNumber - 1) * -.5f * offset; // second part is the same as /2, we just do * 0.5 instead
        float z = -0.0009f;

        for (int i = 0, renderPriority = 0; i < _cardsNumber; i++, renderPriority++)
		{
			GameObject card = PlayerCardManager.Instance.CardsInHand[i];

			float x = start + i * offset;

            // Consider adding parent for pCards
            card.transform.position = new Vector3(x, _hand.gameObject.transform.position.y, _hand.gameObject.transform.position.z + z);
            card.transform.rotation = Quaternion.Euler(200f, card.transform.rotation.y, card.transform.rotation.z);
            card.transform.localScale /= 1.7f;

            card.GetComponent<PlayerCardLogic>().SaveDefaultProperties();
			card.SetActive(true);
            z -= 0.0007f;
            card.transform.SetParent(_hand.gameObject.transform, true);

		}

    }

}
