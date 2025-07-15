using System.Collections.Generic;
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
                Debug.Log($"Card {j} added to suit {i}");
            }

        }   

        // To Test
        CombatStarts();
    }

    public void CombatStarts()
    { 
       EnemyHealth = 50; // Set initial health for the enemy(average enemy?)
       
        //temporary
       PlaceCards(startIndex: 0);
       PlaceCards(startIndex: 3); 
       PlaceCards(startIndex: 6); 
    }

    private void PlaceCards(int startIndex)
    { 
        int cardsInOneSpot = Random.Range(1, 3); // randomly from 1-2

          switch (cardsInOneSpot)
          {
              case 1:
                SpawnSingle(cardObjectIndex: startIndex + 1);
                  break;
              case 2:
                SpawnCombo(indexObject: startIndex);
                  break;
          }

    }

    private void SpawnCombo(int indexObject)
    {
        int cleanOrDirty = Random.Range(1, 3); // randomly from 1-2
        bool isClean = cleanOrDirty == 1;
        Debug.Log($"Is clean: {isClean}");
        int cardsInCombo = 3;

        int randomCleanSuit = Random.Range(0, cardsBySuit.Count);
        int randomDirtySuit = Random.Range(0, cardsBySuit.Count - cardsInCombo);
        int randomCard = Random.Range(0, cardsSameSuit - cardsInCombo);

        for (int i = 0; i < cardsInCombo; i++)
        {
            if (!isClean) break;

            GameObject cardObject = cardObjects[indexObject++];
            CardsSOTest card = cardsBySuit[randomCleanSuit][randomCard++];

            cardObject.GetComponent<SpriteRenderer>().sprite = card.graphics;
            cardObject.SetActive(true);

        }

        for (int i = 0; i < cardsInCombo; i++)
        {
            if (isClean) break;

            GameObject cardObject = cardObjects[indexObject++];
            CardsSOTest card = cardsBySuit[randomDirtySuit++][randomCard];

            cardObject.GetComponent<SpriteRenderer>().sprite = card.graphics;
            cardObject.SetActive(true);

        }

    }

    private void SpawnSingle(int cardObjectIndex)
    {
        int randomSuit = Random.Range(0, cardsBySuit.Count);
        int randomCard = Random.Range(0, cardsSameSuit);


        GameObject cardObject = cardObjects[cardObjectIndex];
        CardsSOTest card = cardsBySuit[randomSuit][randomCard];

        cardObject.GetComponent<SpriteRenderer>().sprite = card.graphics;
        cardObject.SetActive(true);
    }

    public void RefillCards()
    {
        
    }

}
