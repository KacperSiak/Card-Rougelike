using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCardSpawner : MonoBehaviour
{
    public BoxCollider MainHand { get; set; }

    [SerializeField] private float _spawnDuration = 0.5f;
    [SerializeField] private RefillSFX _refillSFX;

    private int _cardsNumber = 0;
    private const int k_maxHandSize = 8; // max without overlay
    private float _cardSize = 0.06f;
    private bool isPlaying = false;

    public void SpawnCards(List<CardsSO> playerDeck, List<GameObject> playerCards,  int cardsAmount)
    {
        for (int i = 0; i < cardsAmount; i++)
        {
            if(playerDeck.Count == 0) break;

            GameObject cardObject = ObjPoolManager.Instance.GetCardFromPool(isSenderPlayer: true);
            playerCards.Add(cardObject);
            PlayerCardManager.Instance.IncreaseCardsInHandAmount();

            CardsSO card = playerDeck[Random.Range(0, playerDeck.Count)];
            playerDeck.Remove(card);

            cardObject.GetComponent<PlayerCardLogic>().SetCardProperties(card);
            MaterialSetupManager.Instance.SetupVisual(cardObject.GetComponent<MeshRenderer>(), card.column, card.row);
        }
        StartPlacement();
    }

    public void StartPlacement()
    {
        if (isPlaying)
        { 
            StopAllCoroutines();
            isPlaying = false;
            MainHand.gameObject.GetComponent<CardHolderAnimation>().EnableAnimations(true);
        }

        _cardsNumber = PlayerCardManager.Instance.CardsInHand.Count;
        if (_cardsNumber == 0) return;

        StartCoroutine(SetCardsPosition());
    }

    private IEnumerator SetCardsPosition()
	{
        MainHand.gameObject.GetComponent<CardHolderAnimation>().DisableAnimations();
        isPlaying = true;

        float offset = _cardSize - _cardSize * (Mathf.Max(k_maxHandSize, _cardsNumber) - k_maxHandSize) / Mathf.Max(_cardsNumber - 1, 1);

        //Same as the above
        //float offset;
        //if (_cardsNumber <= k_maxHandSize)
        //    offset = cardSize;
        //else
        //    offset = cardSize - cardSize * (_cardsNumber - k_maxHandSize) / (float)(_cardsNumber - 1);

		float start = (_cardsNumber - 1) * -.5f * offset; // second part is the same as /2, we just do * 0.5 instead
        float z = -0.0006f;

        for (int i = 0; i < _cardsNumber; i++)
		{
			GameObject card = PlayerCardManager.Instance.CardsInHand[i];
            PlayerCardLogic logic = card.GetComponent<PlayerCardLogic>();

            float x = start + i * offset;
            card.transform.GetPositionAndRotation(out Vector3 startPos, out Quaternion startRot);
            Vector3 startedScale = card.transform.localScale;
            Vector3 finalPos = new (x, MainHand.gameObject.transform.position.y, MainHand.gameObject.transform.position.z + z);
            Vector3 finalScale = card.transform.localScale / 1.7f;
            card.SetActive(true);

            if (logic.IsNew)
            { 
                float elapsed = 0f; 

                _refillSFX.Play();
                while (elapsed < _spawnDuration)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.SmoothStep(0f, 1f, elapsed / _spawnDuration);
                    card.transform.SetPositionAndRotation(Vector3.Lerp(startPos, finalPos, t), Quaternion.Slerp(startRot, MainHand.transform.rotation, t));
                    card.transform.localScale = Vector3.Lerp(startedScale, finalScale, t);
                    yield return null;
                }
            }
            else
                card.transform.SetPositionAndRotation(finalPos, MainHand.transform.rotation);

            logic.SaveDefaultProperties();
            logic.RestoreSelectedPosition();
            z -= 0.0006f;
            card.transform.SetParent(MainHand.gameObject.transform, true);

		}

        MainHand.gameObject.GetComponent<CardHolderAnimation>().EnableAnimations(true);
        isPlaying = false;
    }

}
