using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class EnemyBechavior : MonoBehaviour
{
    public int EnemyHealth { get; private set;}
    [SerializeField] private List<CardsSOTest> enemyCards = new List<CardsSOTest>();
    [SerializeField] private List<GameObject> cardObjects = new List<GameObject>();
    private Dictionary<int, List<CardsSOTest>> cardsBySuit = new Dictionary<int, List<CardsSOTest>>();
    private int cardsSameSuit = 5; 
    private int suitsAmount = 4; 

    void Start()
    {
        for (int i = 0, j = 0; i < suitsAmount; i++)
        {
            cardsBySuit.Add(i, new List<CardsSOTest>());
            int cardsInSuit = cardsSameSuit + j;

            for (; j < cardsInSuit; j++)
            {
                cardsBySuit[i].Add(enemyCards[j]);
            }

        }   

        // To Test
        CombatStarts();
    }

    public void CombatStarts()
    { 
       EnemyHealth = 50; // Set initial health for the enemy(average enemy?)

        //temporary
       PlaceCards(startIndex: 0, activationIndex: 0);
       PlaceCards(startIndex: 3, activationIndex: 1); 
       PlaceCards(startIndex: 6, activationIndex: 2); 
    }

    private void PlaceCards(int startIndex, int activationIndex)
    { 
        int cardsInOneSpot = Random.Range(1, 3); // randomly from 1-2

          switch (cardsInOneSpot)
          {
              case 1:
                SpawnSingle(cardObjectIndex: startIndex + 1, activationIndex);
                  break;
              case 2:
                SpawnCombo(indexObject: startIndex, activationIndex);
                  break;
          }

    }

    private void SpawnCombo(int indexObject, int activationIndex)
    {
        int cleanOrDirty = Random.Range(1, 3); // randomly from 1-2
        bool isClean = cleanOrDirty == 1;
        Debug.Log($"Is clean: {isClean}");
        int cardsInCombo = 3;
        int cardsBuffer = 2;

        int randomCleanSuit = Random.Range(0, cardsBySuit.Count);
        int randomCleanCard = Random.Range(0, cardsBySuit[randomCleanSuit].Count - cardsBuffer); 

        int randomDirtySuit = Random.Range(0, cardsBySuit.Count - cardsBuffer);
        int randomDirtyCard = cardsBySuit[randomDirtySuit][Random.Range(0, cardsBySuit[randomDirtySuit].Count)].power;

        int key = DamageManager.Instance.eCards.Count == 0 ? 0 : DamageManager.Instance.eCards.Count;
        DamageManager.Instance.eCards.Add(key, new List<GameObject>());

        if (isClean)
        {
            switch (cardsBySuit[randomCleanSuit].Count)
            {
                case 1:
                case 2:
                    SpawnSingle(indexObject, activationIndex);
                    return;
            }

            for (int i = 0, j = 1; i < cardsInCombo; i++)
            {
                for (; j < cardsInCombo; j++)
                {
                    Debug.Log($"Card {cardsBySuit[randomCleanSuit][randomCleanCard].power + j} and {cardsBySuit[randomCleanSuit][randomCleanCard + j].power} are in sequence");
                    if (cardsBySuit[randomCleanSuit][randomCleanCard].power + j != cardsBySuit[randomCleanSuit][randomCleanCard + j].power)
                    {
                        PlaceCards(indexObject, activationIndex);
                        return;
                    }
                }

                Debug.Log($"Card {cardsBySuit[randomCleanSuit][randomCleanCard].power} is clean");

                GameObject cardObject = cardObjects[indexObject++];
                CardsSOTest card = cardsBySuit[randomCleanSuit][randomCleanCard];
                cardsBySuit[randomCleanSuit].RemoveAt(randomCleanCard);

                cardObject.GetComponent<SpriteRenderer>().sprite = card.graphics;
                cardObject.GetComponent<EnemyCardLogic>().SetCardProperties(card);
                cardObject.SetActive(true);

                DamageManager.Instance.eCards[key].Add(cardObject);
                DamageManager.Instance.SetCardHolder(activationIndex, type: ComboTypes.Clean);
            }

            if (cardsBySuit[randomCleanSuit].Count == 0)
                cardsBySuit.Remove(randomCleanSuit);
        }

        else
        {
            Debug.Log("I'm here");
            
            if (cardsBySuit.Count < 3)
            {
                PlaceCards(indexObject, activationIndex);
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
                    PlaceCards(indexObject, activationIndex);
                    return;
                }
            }

            Debug.Log("I'm here under for check");

            for (int i = 0; i < cardsInCombo; i++)
            {
                GameObject cardObject = cardObjects[indexObject++];
                CardsSOTest card = cardsBySuit[randomDirtySuit][index[i]]; 
                cardsBySuit[randomDirtySuit].Remove(card);

                if (cardsBySuit[randomDirtySuit].Count == 0)
                    cardsBySuit.Remove(randomDirtySuit);

                randomDirtySuit++;

                cardObject.GetComponent<SpriteRenderer>().sprite = card.graphics;
                cardObject.GetComponent<EnemyCardLogic>().SetCardProperties(card);
                cardObject.SetActive(true);

                DamageManager.Instance.eCards[key].Add(cardObject);
                DamageManager.Instance.SetCardHolder(activationIndex, type: ComboTypes.Dirty);
            }
        }

    }

    private void SpawnSingle(int cardObjectIndex, int activationIndex)
    {
        int randomSuit = Random.Range(0, cardsBySuit.Count);
        int randomCard = Random.Range(0, cardsBySuit[randomSuit].Count);

        Debug.Log($"Random suit: {randomSuit}, Random card: {randomCard}");

        GameObject cardObject = cardObjects[cardObjectIndex];
        CardsSOTest card = cardsBySuit[randomSuit][randomCard];
        cardsBySuit[randomSuit].RemoveAt(randomCard); 

        if (cardsBySuit[randomSuit].Count == 0)
           cardsBySuit.Remove(randomSuit);

        cardObject.GetComponent<SpriteRenderer>().sprite = card.graphics;
        cardObject.GetComponent<EnemyCardLogic>().SetCardProperties(card);
        cardObject.SetActive(true);

        DamageManager.Instance.eCards.Add(DamageManager.Instance.eCards.Count, new List<GameObject> { cardObject });
        DamageManager.Instance.SetCardHolder(activationIndex, type: ComboTypes.Single);
    }

    public void RefillCards()
    {
        
    }

}
