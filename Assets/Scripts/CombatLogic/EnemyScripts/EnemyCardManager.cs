using System.Collections.Generic;
using UnityEngine;

public class EnemyCardManager : MonoBehaviour
{
    public static EnemyCardManager Instance { get; private set; }

    [SerializeField] private EnemyCardSpawner _spawner;
    
    public List<GameObject> cardHolders;
    public List<GameObject> cardObjects;

    private Dictionary<int, List<CardsSOTest>> _cardsBySuit = new Dictionary<int, List<CardsSOTest>>();

    public Dictionary<int, List<GameObject>> CardsInGame { get; private set;} //update
    public Dictionary<int, int> CardsInGamePower { get; private set; } //update

    private int _suitsAmount = 4;
    private int _cardsSameSuit = 5;

    void Awake()
    {
        if (Instance == null) Instance = this;

        else Destroy(gameObject);
    }

    public void StartCombat(List<CardsSOTest> enemyCards)
    {
        for (int i = 0, j = 0; i < _suitsAmount; i++)
        {
            _cardsBySuit.Add(i, new List<CardsSOTest>());
            int cardsInSuit = _cardsSameSuit + j;

            for (; j < cardsInSuit; j++)
            {
                _cardsBySuit[i].Add(enemyCards[j]);
            }

        }

        CardsInGame = new Dictionary<int, List<GameObject>>();
        CardsInGamePower = new Dictionary<int, int>();

        for (int i = 0, j = 0; i < 3; i++)
        {
            SetupSpawner(startObjectIndex: j, cardHolderIndex: i);
            j += 3;
        }

    }

    public void SetupSpawner(int startObjectIndex, int cardHolderIndex)
    {
        bool isSingle = Random.Range(0, 2) == 0; // randomly choose between single or combo, 0 is single(true), 1 is combo(false)

        switch (isSingle)
        {
            case true:
                _spawner.SpawnSingle(startObjectIndex + 1, cardHolderIndex, _cardsBySuit);
                break;
            case false:
                _spawner.SpawnCombo(startObjectIndex, cardHolderIndex, _cardsBySuit);
                break;
        }
    }

    public void SetupCardHolder(int index, IValidate validationType)
    {
        cardHolders[index].GetComponent<CardHolderManager>().SetProperties(validationType, index);
    }

    public void SetupCardsInGame(int key)
    { 
        CardsInGame.Add(key, new List<GameObject>());
    }

    public void UpdateCardsInGame(int key, GameObject cardObject)
    { 
        CardsInGame[key].Add(cardObject);
    }

    public void RemoveCardFromGame(int key)
    {
        CardsInGame.Remove(key);
        CardsInGamePower.Remove(key);
    }

    public void SetupPower(int keyCardsIndex, int power)
    {
        CardsInGamePower.Add(keyCardsIndex, power);
    }

    public void UpdatePower(int keyCardsIndex, int power)
    {
        CardsInGamePower[keyCardsIndex] -= power;
    }

    public void RefillCards()
    {
        int startIndex = 0;

        foreach (var cardHolder in cardHolders)
        {
            if (cardHolder.GetComponent<CardHolderManager>().EnemyComboIndex != -1)
            {
                startIndex += 3;
                continue;
            } 

            SetupSpawner(startObjectIndex: startIndex, cardHolderIndex: cardHolders.IndexOf(cardHolder));
            startIndex += 3;
        }
    }
}
