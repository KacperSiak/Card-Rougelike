using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackManager : MonoBehaviour
{
    [SerializeField] public Dictionary<int, int> RoundsForCards { get; private set; } = new();
    private int _round;
    private const int minTotalPower = 2;
    private const int maxTotalPower = 42;
    private const int maxRoundsToAttack = 5;
    private const int minRound = 1;
    private const int maxRound = 5;
    private const int limit = 3;

    private void OnEnable()
    {
        RoundManager.OnCombatEnd += OnCombatEnd;
        RoundManager.OnCombatStart += StartCombatSetup;
    }


    private void OnDisable()
    {
        RoundManager.OnCombatEnd -= OnCombatEnd;
        RoundManager.OnCombatStart -= StartCombatSetup;
    }

    public void StartCombatSetup()
    {
        RoundsForCards.Clear();
    }

    public void SetRoundAttack(int key)
    {
        int _cardsPower = EnemyCardManager.Instance.CardsInGamePower[key];

        _round = CalculateRound(_cardsPower);

        RoundsForCards.Add(key, _round);

        UIManager.Instance.SetRoundForEnemyUI(key);
    }

    private int CalculateRound(int totalPower)
    { 
        int amount = Mathf.CeilToInt(maxRoundsToAttack * (totalPower - minTotalPower) / (maxTotalPower - minTotalPower));

        //return Mathf.Clamp(amount, minRound, maxRound);
        return 100;
    }

    public void UpdateRounds()
    {
        for (int i = 0; i < limit; i++)
        {
            if (!RoundsForCards.ContainsKey(i))
            {
                UIManager.Instance.SetRoundForEnemyUI(key: i);
                continue;
            } 

            RoundsForCards[i]--;
            UIManager.Instance.SetRoundForEnemyUI(key: i);

            if (RoundsForCards[i] > 0) continue;

            RoundManager.Instance.PlayerHP -= EnemyCardManager.Instance.CardsInGamePower[i];

            if (RoundManager.Instance.PlayerHP <= 0) return;

            RemoveKey(i);
            foreach (var card in EnemyCardManager.Instance.CardsInGame[i])
            {
                card.SetActive(false);
            }
            EnemyCardManager.Instance.RemoveCardFromGame(i);
            EnemyCardManager.Instance.cardHolders[i].GetComponent<CardHolderManager>().ResetCardHolderEnemyCards();
        }
    }

    public void RemoveKey(int key)
    { 
        RoundsForCards.Remove(key);
    }

    private void OnCombatEnd()
    { 
        RoundsForCards.Clear();
    }
}
