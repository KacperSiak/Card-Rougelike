using System.Collections.Generic;
using UnityEngine;

public class EnemyCardSpawner : MonoBehaviour
{
    private int _cardsInCombo = 3;
    private int _cardsBuffer = 2;

    public void SpawnCombo(int indexObject, int indexCardHolder, Dictionary<int, List<CardsSOTest>> cardsBySuit)
    {
        int cleanOrDirty = Random.Range(0, 2); // randomly from 0-1, 0 is clean, 1 is dirty
        bool isClean = cleanOrDirty == 0;

        Debug.Log($"Is clean: {isClean}");

        int randomCleanSuit = Random.Range(0, cardsBySuit.Count);
        int randomCleanCard = Random.Range(0, cardsBySuit[randomCleanSuit].Count - _cardsBuffer);

        int randomDirtySuit = Random.Range(0, cardsBySuit.Count - _cardsBuffer);
        int randomDirtyCard = cardsBySuit[randomDirtySuit][Random.Range(0, cardsBySuit[randomDirtySuit].Count)].power;

        int key = EnemyCardManager.Instance.CardsInGame.Count == 0 ? 0 : EnemyCardManager.Instance.CardsInGame.Count;
        EnemyCardManager.Instance.SetupCardsInGame(key);

        if (isClean)
        {
            switch (cardsBySuit[randomCleanSuit].Count)
            {
                case 1:
                case 2:
                    SpawnSingle(indexObject, indexCardHolder, cardsBySuit);
                    return;
            }

            for (int i = 0, j = 1; i < _cardsInCombo; i++)
            {
                for (; j < _cardsInCombo; j++)
                {
                    Debug.Log($"Card {cardsBySuit[randomCleanSuit][randomCleanCard].power + j} and {cardsBySuit[randomCleanSuit][randomCleanCard + j].power} are in sequence");
                    if (cardsBySuit[randomCleanSuit][randomCleanCard].power + j != cardsBySuit[randomCleanSuit][randomCleanCard + j].power)
                    {
                        EnemyCardManager.Instance.SetupSpawner(indexObject, indexCardHolder);
                        return;
                    }
                }

                Debug.Log($"Card {cardsBySuit[randomCleanSuit][randomCleanCard].power} is clean");

                GameObject cardObject = EnemyCardManager.Instance.CardObjects[indexObject++];
                CardsSOTest card = cardsBySuit[randomCleanSuit][randomCleanCard];
                cardsBySuit[randomCleanSuit].RemoveAt(randomCleanCard);

                cardObject.GetComponent<SpriteRenderer>().sprite = card.graphics;
                cardObject.GetComponent<EnemyCardLogic>().SetCardProperties(card);
                cardObject.SetActive(true);

                EnemyCardManager.Instance.UpdateCardsInGame(key, cardObject);
                EnemyCardManager.Instance.SetupCardHolder(indexCardHolder, type: ComboTypes.Clean);
            }

            if (cardsBySuit[randomCleanSuit].Count == 0)
                cardsBySuit.Remove(randomCleanSuit);
        }

        else
        {
            Debug.Log("I'm here");

            if (cardsBySuit.Count < 3)
            {
                EnemyCardManager.Instance.SetupSpawner(indexObject, indexCardHolder);
                return;
            }

            List<int> index = new List<int>();

            Debug.Log("I'm here under count");

            for (int i = randomDirtySuit, count = 1; i < randomDirtySuit + 3; i++, count++) //here is a problem
            {
                foreach (var card in cardsBySuit[i])
                {
                    if (card.power == randomDirtyCard)
                    {
                        index.Add(cardsBySuit[i].IndexOf(card));
                        break;
                    }
                }

                if (index.Count != count)
                {
                    EnemyCardManager.Instance.SetupSpawner(indexObject, indexCardHolder);
                    return;
                }
            }

            Debug.Log("I'm here under for check");

            for (int i = 0; i < _cardsInCombo; i++)
            {
                GameObject cardObject = EnemyCardManager.Instance.CardObjects[indexObject++];
                CardsSOTest card = cardsBySuit[randomDirtySuit][index[i]];
                cardsBySuit[randomDirtySuit].Remove(card);

                if (cardsBySuit[randomDirtySuit].Count == 0)
                    cardsBySuit.Remove(randomDirtySuit);

                randomDirtySuit++;

                cardObject.GetComponent<SpriteRenderer>().sprite = card.graphics;
                cardObject.GetComponent<EnemyCardLogic>().SetCardProperties(card);
                cardObject.SetActive(true);

                EnemyCardManager.Instance.UpdateCardsInGame(key, cardObject);
                EnemyCardManager.Instance.SetupCardHolder(indexCardHolder, type: ComboTypes.Dirty);
            }
        }

    }

    public void SpawnSingle(int cardObjectIndex, int activationIndex, Dictionary<int, List<CardsSOTest>> cardsBySuit)
    {
        int randomSuit = Random.Range(0, cardsBySuit.Count);
        int randomCard = Random.Range(0, cardsBySuit[randomSuit].Count);

        Debug.Log($"Random suit: {randomSuit}, Random card: {randomCard}");

        GameObject cardObject = EnemyCardManager.Instance.CardObjects[cardObjectIndex];
        CardsSOTest card = cardsBySuit[randomSuit][randomCard];
        cardsBySuit[randomSuit].RemoveAt(randomCard);

        if (cardsBySuit[randomSuit].Count == 0)
            cardsBySuit.Remove(randomSuit);

        cardObject.GetComponent<SpriteRenderer>().sprite = card.graphics;
        cardObject.GetComponent<EnemyCardLogic>().SetCardProperties(card);
        cardObject.SetActive(true);
            
        int keySingle = EnemyCardManager.Instance.CardsInGame.Count;
        EnemyCardManager.Instance.SetupCardsInGame(keySingle);
        EnemyCardManager.Instance.UpdateCardsInGame(keySingle, cardObject);
        EnemyCardManager.Instance.SetupCardHolder(activationIndex, type: ComboTypes.Single);
    }
}
