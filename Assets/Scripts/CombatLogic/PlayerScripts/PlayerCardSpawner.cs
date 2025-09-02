using System.Collections.Generic;
using UnityEngine;

public class PlayerCardSpawner : MonoBehaviour
{
    public BoxCollider MainHand { get; set; }

    private int _cardsNumber = 0;
    private const int k_maxHandSize = 8; // max without overlay
    private float _cardSize = 0.06f;

    public void SpawnCards(List<CardsSOTest> playerDeck, List<GameObject> playerCards,  int cardsAmount)
    {
        for (int i = 0; i < cardsAmount; i++)
        {
            if(playerDeck.Count == 0) break;

            GameObject cardObject = ObjPoolManager.Instance.GetCardFromPool(isSenderPlayer: true);
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
		MainHand.gameObject.GetComponent<HandAnimationLogic>().DisableAnimations();

        _cardsNumber = PlayerCardManager.Instance.CardsInHand.Count;
		if (_cardsNumber == 0) return;

		float handSize = MainHand.bounds.size.x;

        float offset = _cardSize - _cardSize * (Mathf.Max(k_maxHandSize, _cardsNumber) - k_maxHandSize) / Mathf.Max(_cardsNumber - 1, 1);

        //Same as the above
        //float offset;
        //if (_cardsNumber <= k_maxHandSize)
        //    offset = cardSize;
        //else
        //    offset = cardSize - cardSize * (_cardsNumber - k_maxHandSize) / (float)(_cardsNumber - 1);

		float start = (_cardsNumber - 1) * -.5f * offset; // second part is the same as /2, we just do * 0.5 instead
        float z = -0.0009f; //(+/-)

        for (int i = 0; i < _cardsNumber; i++)
		{
			GameObject card = PlayerCardManager.Instance.CardsInHand[i];

			float x = start + i * offset;

            // Consider adding parent for pCards
            card.transform.position = new Vector3(x, MainHand.gameObject.transform.position.y, MainHand.gameObject.transform.position.z + z);
            card.transform.rotation = MainHand.transform.rotation;

            card.GetComponent<PlayerCardLogic>().SaveDefaultProperties();
			card.SetActive(true);
            z -= 0.0007f;
            card.transform.SetParent(MainHand.gameObject.transform, true);

		}

        MainHand.gameObject.GetComponent<HandAnimationLogic>().EnableAnimations();

    }

}
