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

        int randomCleanSuit = Random.Range(0, cardsBySuit.Count);
        int randomDirtySuit = Random.Range(0, cardsBySuit.Count - cardsInCombo);
        int randomCard = Random.Range(0, cardsSameSuit - cardsInCombo);
        int key = DamageManager.Instance.eCards.Count == 0 ? 0 : DamageManager.Instance.eCards.Count;
        DamageManager.Instance.eCards.Add(key, new List<GameObject>());


        for (int i = 0; i < cardsInCombo; i++)
        {
            if (!isClean) break;

            GameObject cardObject = cardObjects[indexObject++];
            CardsSOTest card = cardsBySuit[randomCleanSuit][randomCard++];

            cardObject.GetComponent<SpriteRenderer>().sprite = card.graphics;
            cardObject.GetComponent<EnemyCardLogic>().SetCardProperties(card);
            cardObject.SetActive(true);

            DamageManager.Instance.eCards[key].Add(cardObject);
            DamageManager.Instance.SetCardHolder(activationIndex, type: ComboTypes.Clean);
        }

        for (int i = 0; i < cardsInCombo; i++)
        {
            if (isClean) break;

            GameObject cardObject = cardObjects[indexObject++];
            CardsSOTest card = cardsBySuit[randomDirtySuit++][randomCard];

            cardObject.GetComponent<SpriteRenderer>().sprite = card.graphics;
            cardObject.GetComponent<EnemyCardLogic>().SetCardProperties(card);
            cardObject.SetActive(true);

            DamageManager.Instance.eCards[key].Add(cardObject);
            DamageManager.Instance.SetCardHolder(activationIndex, type: ComboTypes.Dirty);
        }

    }

    private void SpawnSingle(int cardObjectIndex, int activationIndex)
    {
        int randomSuit = Random.Range(0, cardsBySuit.Count);
        int randomCard = Random.Range(0, cardsSameSuit);


        GameObject cardObject = cardObjects[cardObjectIndex];
        CardsSOTest card = cardsBySuit[randomSuit][randomCard];

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
