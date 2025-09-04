using System.Collections.Generic;
using UnityEngine;

public class EnemyCardManager : MonoBehaviour
{
    public static EnemyCardManager Instance { get; private set; }

    [SerializeField] private EnemyCardSpawner _spawner;
    [SerializeField] private List<EnemyCardAnimation> _animations; 

    public List<GameObject> cardHolders;
    public List<Transform> cardObjects;

    private Dictionary<int, List<CardsSOTest>> _cardsBySuitDeck = new();

    public Dictionary<int, List<GameObject>> CardsInGame { get; private set;} 
    public Dictionary<int, int> CardsInGamePower { get; private set; }

    public int CardsInGameCount { get; private set; } = 0;
    private int _bittenCards = 0;

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
        _cardsBySuitDeck.Clear();
        _cardsBySuitDeck = CombatManager.Instance.GetCurrentDeck();

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

    public void ControlIdleAnimation()
    {
        foreach (var a in _animations)
        {
            if (a.IsAnimating) return;
        }

        foreach (var cardHolder in cardObjects)
        {
            cardHolder.GetComponent<CardHolderAnimation>().EnableAnimations(isPHand: false);
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
        CardsInGameCount++;
    }

    public void RemoveCardFromGame(int key, int amount)
    {
        foreach (var cardHolder in cardObjects)
        {
            cardHolder.GetComponent<CardHolderAnimation>().DisableAnimations();
        }
        _animations[key].RemoveCard(CardsInGame[key].ConvertAll(card => card.transform).ToArray());
        CardsInGame.Remove(key);
        CardsInGamePower.Remove(key);
        _bittenCards++; 
        CardsInGameCount -= amount;
    }

    public void SetupPower(int keyCardsIndex, int power)
    {
        CardsInGamePower.Add(keyCardsIndex, power);
    }

    public void UpdatePower(int keyCardsIndex, int power)
    {
        CardsInGamePower[keyCardsIndex] -= power;
    }

    public bool CheckRoundReadiness() // here improve logic later
    {
        if(_cardsBySuitDeck.Count == 0 && CardsInGame.Count == 0) return false; 

        else if (_bittenCards >= RoundManager.Instance.CurrentCombatDetails.key) return false;

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

    //??
    private void OnCombatEnd() //here improve
    {
        foreach (var card in cardObjects)
        {
            card.gameObject.SetActive(false);
        }
        CardsInGame.Clear();
        CardsInGamePower.Clear();
    }
}
