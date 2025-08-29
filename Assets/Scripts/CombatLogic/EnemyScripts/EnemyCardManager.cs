using System.Collections.Generic;
using UnityEngine;

public class EnemyCardManager : MonoBehaviour
{
    public static EnemyCardManager Instance { get; private set; }

    [SerializeField] private EnemyCardSpawner _spawner;
    
    public List<GameObject> cardHolders;
    public List<Transform> cardObjects;

    private Dictionary<int, List<CardsSOTest>> _cardsBySuitDeck = new Dictionary<int, List<CardsSOTest>>();

    public Dictionary<int, List<GameObject>> CardsInGame { get; private set;} 
    public Dictionary<int, int> CardsInGamePower { get; private set; } 

    private int _suitsAmount = 4;
    private int _cardsSameSuit = 5;

    void Awake()
    {
        if (Instance == null) Instance = this;

        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        RoundManager.OnCombatEnd += OnCombatEnd;
        RoundManager.OnRoundEnd += RefillCards;
        RoundManager.OnCombatStart += StartCombat;
    }


    private void OnDisable()
    {
        RoundManager.OnCombatEnd -= OnCombatEnd;
        RoundManager.OnRoundEnd -= RefillCards;
        RoundManager.OnCombatStart -= StartCombat;
    }

    public void StartCombat()
    {
        for (int i = 0, j = 0; i < _suitsAmount; i++)
        {
            _cardsBySuitDeck.Add(i, new List<CardsSOTest>());
            int cardsInSuit = _cardsSameSuit + j;

            for (; j < cardsInSuit; j++)
            {
                _cardsBySuitDeck[i].Add(RoundManager.Instance.EnemyCards[j]);
            }

        }

        CardsInGame = new Dictionary<int, List<GameObject>>();
        CardsInGamePower = new Dictionary<int, int>();

        for (int i = 0; i < 3; i++)
        {
            SetupSpawner(startObjectIndex: i, cardHolderIndex: i);
        }
    }

    public void SetupSpawner(int startObjectIndex, int cardHolderIndex)
    {
        if (_cardsBySuitDeck.Count == 0) return;

        bool isSingle = Random.Range(0, 2) == 0; // randomly choose between single or combo, 0 is single(true), 1 is combo(false)

        switch (isSingle)
        {
            case true:
                _spawner.SpawnSingle(cardObjects[startObjectIndex], cardHolderIndex, _cardsBySuitDeck);
                break;
            case false:
                _spawner.SpawnCombo(cardObjects[startObjectIndex], cardHolderIndex, _cardsBySuitDeck);
                break;
        }
    }

    public void SetupCardHolder(int index, Validator validator)
    {
        cardHolders[index].GetComponent<CardHolderLogic>().SetProperties(validator, index);
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

    public bool CheckRoundReadiness()
    {
        if(_cardsBySuitDeck.Count == 0 && CardsInGame.Count == 0) return false;
        
        return true;
    }

    private void RefillCards()
    {
        if(_cardsBySuitDeck.Count == 0) return;

        int startIndex = 0;

        foreach (var cardHolder in cardHolders)
        {
            if (cardHolder.GetComponent<CardHolderLogic>().EnemyComboIndex != -1)
            {
                startIndex++;
                continue;
            } 

            SetupSpawner(startObjectIndex: startIndex, cardHolderIndex: cardHolders.IndexOf(cardHolder));
            startIndex++;
        }
    }

    private void OnCombatEnd()
    {
        foreach (var card in cardObjects)
        {
            card.gameObject.SetActive(false);
        }
        CardsInGame.Clear();
        CardsInGamePower.Clear();
    }
}
