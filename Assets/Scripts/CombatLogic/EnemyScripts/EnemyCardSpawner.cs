using System.Collections.Generic;
using UnityEngine;

public class EnemyCardSpawner : MonoBehaviour
{
    [SerializeField] EnemyAttackManager _enemyAttackManager;
    [SerializeField] private ValidateClean _clean;
    [SerializeField] private ValidateDirty _dirty;
    [SerializeField] private ValidateSingle _single;
    private int _cardsInCombo = 3;
    private int _cardsBuffer = 2;

    public void SpawnCombo(int indexObject, int indexCardHolder, Dictionary<int, List<CardsSOTest>> cardsBySuit)
    {  
        int cleanOrDirty = Random.Range(0, 2); // randomly from 0-1, 0 is clean, 1 is dirty
        bool isClean = cleanOrDirty == 0;

        if (!EnemyCardManager.Instance.CardsInGame.ContainsKey(indexCardHolder))
        {
            EnemyCardManager.Instance.SetupCardsInGame(indexCardHolder);
        }

        int key = indexCardHolder;

        int power = 0;

        if (isClean)
        {
            int randomCleanSuit = Random.Range(0, cardsBySuit.Count);

            if (cardsBySuit[randomCleanSuit].Count <= _cardsBuffer)
            {
                SpawnSingle(indexObject + 1, indexCardHolder, cardsBySuit);
                return;
            }

            int randomCleanCard = Random.Range(0, cardsBySuit[randomCleanSuit].Count - _cardsBuffer);

            for (int i = 0, j = 1; i < _cardsInCombo; i++)
            {
                for (; j < _cardsInCombo; j++)
                { 
                    if (cardsBySuit[randomCleanSuit][randomCleanCard].power + j != cardsBySuit[randomCleanSuit][randomCleanCard + j].power)
                    {
                        EnemyCardManager.Instance.SetupSpawner(indexObject, indexCardHolder);
                        return;
                    }
                }

                GameObject cardObject = EnemyCardManager.Instance.cardObjects[indexObject + i];
                CardsSOTest card = cardsBySuit[randomCleanSuit][randomCleanCard];
                cardsBySuit[randomCleanSuit].RemoveAt(randomCleanCard);

                cardObject.GetComponent<SpriteRenderer>().sprite = card.graphics;
                cardObject.GetComponent<EnemyCardLogic>().SetCardProperties(card);
                power += card.power;
                cardObject.SetActive(true);

                EnemyCardManager.Instance.UpdateCardsInGame(key, cardObject);
            }

            CheckCardsInDeckAmount(cardsBySuit, randomCleanSuit);

            EnemyCardManager.Instance.SetupCardHolder(key, validationType: _clean);
            EnemyCardManager.Instance.SetupPower(key, power);
            _enemyAttackManager.SetRoundAttack(key);
            return;
        }

        if (cardsBySuit.Count <= _cardsBuffer)
        {
             EnemyCardManager.Instance.SetupSpawner(indexObject, indexCardHolder);
             return;
        }

        int randomDirtySuit = Random.Range(0, cardsBySuit.Count - _cardsBuffer);
        int randomDirtyCard = cardsBySuit[randomDirtySuit][Random.Range(0, cardsBySuit[randomDirtySuit].Count)].power;

        List<int> index = new List<int>();

        for (int i = randomDirtySuit, count = 1; i < randomDirtySuit + 3; i++, count++) 
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

        for (int i = 0; i < _cardsInCombo; i++)
        {
             GameObject cardObject = EnemyCardManager.Instance.cardObjects[indexObject + i];
             CardsSOTest card = cardsBySuit[randomDirtySuit][index[i]];
             cardsBySuit[randomDirtySuit].Remove(card);

            if (cardsBySuit[randomDirtySuit].Count == 0)
            {
                CheckCardsInDeckAmount(cardsBySuit, randomDirtySuit);
                randomDirtySuit--;
            }

            randomDirtySuit++;

             cardObject.GetComponent<SpriteRenderer>().sprite = card.graphics;
             cardObject.GetComponent<EnemyCardLogic>().SetCardProperties(card);
             power += card.power;
             cardObject.SetActive(true);

             EnemyCardManager.Instance.UpdateCardsInGame(key, cardObject);
        }

        EnemyCardManager.Instance.SetupCardHolder(key, validationType: _dirty);
        EnemyCardManager.Instance.SetupPower(key, power);
        _enemyAttackManager.SetRoundAttack(key);
    }

    public void SpawnSingle(int cardObjectIndex, int indexCardHolder, Dictionary<int, List<CardsSOTest>> cardsBySuit)
    { 
        int randomSuit = Random.Range(0, cardsBySuit.Count);
        int randomCard = Random.Range(0, cardsBySuit[randomSuit].Count); 
        int power = 0;

        GameObject cardObject = EnemyCardManager.Instance.cardObjects[cardObjectIndex];
        CardsSOTest card = cardsBySuit[randomSuit][randomCard]; 
        cardsBySuit[randomSuit].RemoveAt(randomCard);

        CheckCardsInDeckAmount(cardsBySuit, randomSuit);

        cardObject.GetComponent<SpriteRenderer>().sprite = card.graphics;
        cardObject.GetComponent<EnemyCardLogic>().SetCardProperties(card);
        power += card.power;
        cardObject.SetActive(true);
            
        int keySingle = indexCardHolder;
        if (!EnemyCardManager.Instance.CardsInGame.ContainsKey(keySingle))
            EnemyCardManager.Instance.SetupCardsInGame(keySingle);

        EnemyCardManager.Instance.UpdateCardsInGame(keySingle, cardObject);
        EnemyCardManager.Instance.SetupCardHolder(keySingle, validationType: _single);
        EnemyCardManager.Instance.SetupPower(keySingle, power);
        _enemyAttackManager.SetRoundAttack(keySingle);
    }

    private void CheckCardsInDeckAmount(Dictionary<int, List<CardsSOTest>> deck, int key)
    {
        if (deck[key].Count == 0)
        {
            if (!deck.ContainsKey(key + 1))
                deck.Remove(key);

            else
            {
                int emptyKey = 0;

                for (int i = key + 1; i < deck.Count; i++)
                {
                    deck[i - 1] = deck[i];
                    emptyKey = i;
                }

                deck.Remove(emptyKey);
            }
        }
    }
}
