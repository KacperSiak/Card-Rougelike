using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCardSpawner : MonoBehaviour
{
    [SerializeField] EnemyAttackManager _enemyAttackManager;
    [SerializeField] private ValidateClean _clean;
    [SerializeField] private ValidateDirty _dirty;
    [SerializeField] private ValidateSingle _single;
    private readonly int _cardsInCombo = 3;
    private readonly int _cardsBuffer = 2;

    public void SpawnCombo(Transform indexObject, int indexCardHolder, Dictionary<int, List<CardsSOTest>> cardsBySuit)
    {
        if (EnemyCardManager.Instance.CardsInGameCount >= RoundManager.Instance.CurrentCombatDetails.value)
        {
            SpawnSingle(indexObject, indexCardHolder, cardsBySuit);
            return;
        }
        
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
                SpawnSingle(indexObject, indexCardHolder, cardsBySuit);
                return;
            }

            int randomCleanCard = Random.Range(0, cardsBySuit[randomCleanSuit].Count - _cardsBuffer);

            for (int i = 0, j = 1; i < _cardsInCombo; i++)
            {
                for (; j < _cardsInCombo; j++)
                { 
                    if (cardsBySuit[randomCleanSuit][randomCleanCard].power + j != cardsBySuit[randomCleanSuit][randomCleanCard + j].power)
                    {
                        EnemyCardManager.Instance.SetupSpawner(indexCardHolder, indexCardHolder);
                        return;
                    }
                }

                GameObject cardObject = ObjPoolManager.Instance.GetCardFromPool(isSenderPlayer: false);
                CardsSOTest card = cardsBySuit[randomCleanSuit][randomCleanCard];
                cardsBySuit[randomCleanSuit].RemoveAt(randomCleanCard);

                CardSetupManager.Instance.SetupCardVisual(cardObject.GetComponent<MeshRenderer>(), card.column, card.row);
                cardObject.GetComponent<EnemyCardLogic>().SetCardProperties(card);
                power += card.power;

                EnemyCardManager.Instance.UpdateCardsInGame(key, cardObject);
            }

            CheckCardsInDeckAmount(cardsBySuit, randomCleanSuit);

            EnemyCardManager.Instance.SetupCardHolder(key, validator: _clean);
            EnemyCardManager.Instance.SetupPower(key, power);
            _enemyAttackManager.SetRoundAttack(key);
            StartCoroutine(AnimatedPlacement(indexObject, _cardsInCombo, key, 1f));
            return;
        }

        if (cardsBySuit.Count <= _cardsBuffer)
        {
             EnemyCardManager.Instance.SetupSpawner(indexCardHolder, indexCardHolder);
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
                    EnemyCardManager.Instance.SetupSpawner(indexCardHolder, indexCardHolder);
                    return;
              }
        }

        for (int i = 0; i < _cardsInCombo; i++)
        {
            GameObject cardObject = ObjPoolManager.Instance.GetCardFromPool(isSenderPlayer: false);

            CardsSOTest card = cardsBySuit[randomDirtySuit][index[i]];
             cardsBySuit[randomDirtySuit].Remove(card);

            if (cardsBySuit[randomDirtySuit].Count == 0)
            {
                CheckCardsInDeckAmount(cardsBySuit, randomDirtySuit);
                randomDirtySuit--;
            }

            randomDirtySuit++;

            CardSetupManager.Instance.SetupCardVisual(cardObject.GetComponent<MeshRenderer>(), card.column, card.row);
            cardObject.GetComponent<EnemyCardLogic>().SetCardProperties(card);
            power += card.power;

            EnemyCardManager.Instance.UpdateCardsInGame(key, cardObject);
        }

        EnemyCardManager.Instance.SetupCardHolder(key, validator: _dirty);
        EnemyCardManager.Instance.SetupPower(key, power);
        _enemyAttackManager.SetRoundAttack(key);
        StartCoroutine(AnimatedPlacement(indexObject, _cardsInCombo, key, 1f));
    }

    public void SpawnSingle(Transform cardObjectIndex, int indexCardHolder, Dictionary<int, List<CardsSOTest>> cardsBySuit)
    { 
        int randomSuit = Random.Range(0, cardsBySuit.Count);
        int randomCard = Random.Range(0, cardsBySuit[randomSuit].Count); 
        int power = 0;

        GameObject cardObject = ObjPoolManager.Instance.GetCardFromPool(isSenderPlayer: false);
        CardsSOTest card = cardsBySuit[randomSuit][randomCard]; 
        cardsBySuit[randomSuit].RemoveAt(randomCard);

        CheckCardsInDeckAmount(cardsBySuit, randomSuit);

        CardSetupManager.Instance.SetupCardVisual(cardObject.GetComponent<MeshRenderer>(), card.column, card.row);
        cardObject.GetComponent<EnemyCardLogic>().SetCardProperties(card);
        power += card.power;
            
        int keySingle = indexCardHolder;
        if (!EnemyCardManager.Instance.CardsInGame.ContainsKey(keySingle))
            EnemyCardManager.Instance.SetupCardsInGame(keySingle);

        EnemyCardManager.Instance.UpdateCardsInGame(keySingle, cardObject);
        EnemyCardManager.Instance.SetupCardHolder(keySingle, validator: _single);
        EnemyCardManager.Instance.SetupPower(keySingle, power);
        _enemyAttackManager.SetRoundAttack(keySingle);

        StartCoroutine(AnimatedPlacement(cardObjectIndex, 1, keySingle, 1f));
    }

    private IEnumerator AnimatedPlacement(Transform parentPosition, int amount, int key, float _duration)
    {
        Vector3 spawnPosition;

        if (amount == 1)
            spawnPosition = parentPosition.position;
        else
            spawnPosition = parentPosition.position - new Vector3(0.1f, 0, 0);

        while(!ObjPoolManager.Instance.IsDeckReady)
            yield return null;

        for (int i = 0; i < amount; i++)
        {
            GameObject cardObject = EnemyCardManager.Instance.CardsInGame[key][i];
            cardObject.SetActive(true);
            cardObject.transform.localScale /= 1.7f;
            cardObject.transform.GetPositionAndRotation(out Vector3 startPos, out Quaternion startRot);
            float elapsed = 0f;

            while (elapsed < _duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / _duration);
                cardObject.transform.SetPositionAndRotation(Vector3.Lerp(startPos, spawnPosition, t), Quaternion.Slerp(startRot, parentPosition.transform.rotation, t));
                yield return null;
            }
            spawnPosition += new Vector3(0.1f, 0, 0);
        }
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
