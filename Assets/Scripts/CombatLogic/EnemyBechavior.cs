using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBechavior : MonoBehaviour
{
    public int EnemyHealth { get; private set;}
    [SerializeField] private List<CardsSOTest> enemyCards = new List<CardsSOTest>();
    [SerializeField] private List<GameObject> cardObjects = new List<GameObject>();

    void Start()
    {
        // To Test
        CombatStarts();
    }

    public void CombatStarts()
    { 
       EnemyHealth = 50; // Set initial health for the enemy(average enemy?)
       
        //temporary
       PlaceCards(startIndex: 0);
       PlaceCards (startIndex: 3); 
       PlaceCards(startIndex: 6); 
    }

    private void PlaceCards(int startIndex)
    { 
        int cardsInOneSpot = Random.Range(1, 4); // randomly from 1-3

          for (int i = 0; i < cardsInOneSpot; i++)
          {
                GameObject cardObject = cardObjects[startIndex++];
                CardsSOTest card = enemyCards[Random.Range(0, enemyCards.Count)];

                cardObject.GetComponent<MeshRenderer>().material = card.cardMaterial;
                cardObject.SetActive(true);

          }

    }

    public void RefillCards()
    {
        
    }

}
