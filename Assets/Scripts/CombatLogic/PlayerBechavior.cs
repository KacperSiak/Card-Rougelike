using System.Collections.Generic;
using UnityEngine;

public class PlayerBechavior : MonoBehaviour
{
    //TO DO: Combine card spawn logic, move it to a separate script and use it for both player and enemy
    //TO DO: add animations for card movement both for player and enemy

    public int PlayerHealth { get; private set; }
    private int cardsInHand = 6; // Number of cards in hand
    [SerializeField] private List<CardsSOTest> playerCards = new List<CardsSOTest>();
    private List<CardsSOTest> playerCardsInGame;
    [SerializeField] private List<GameObject> cardObjects = new List<GameObject>();

    void Start()
    {
        // To Test
        CombatStarts();
        Cursor.visible = true; // Here for now 
    }

    public void CombatStarts()
    {
        PlayerHealth = 50; // Set initial health for the player 

        //temporary
        playerCardsInGame = new List<CardsSOTest>(playerCards);
        SetPlayerCards();
    }

    private void SetPlayerCards()
    {
        for (int i = 0; i < cardsInHand; i++)
        {
            GameObject cardObject = cardObjects[i];
            CardsSOTest card = playerCardsInGame[Random.Range(0, playerCardsInGame.Count)];
            playerCardsInGame.Remove(card); 

            cardObject.GetComponent<SpriteRenderer>().sprite = card.graphics;
            cardObject.GetComponent<PlayerCardLogic>().SetCardProperties(card);
            cardObject.SetActive(true);
        }
    }

    public void RefillCards()
    { 
    
    }
}
