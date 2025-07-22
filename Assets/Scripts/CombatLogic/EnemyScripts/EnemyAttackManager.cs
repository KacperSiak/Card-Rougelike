using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

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

        return Mathf.Clamp(amount, minRound, maxRound);
    }

    public void UpdateRounds()
    {
        for (int i = 0; i < limit; i++)
        {
            if (!RoundsForCards.ContainsKey(i)) continue;

            RoundsForCards[i]--;
            if (RoundsForCards[i] > 0)
            {
                UIManager.Instance.SetRoundForEnemyUI(key: i);
                continue;
            } 

            RoundManager.Instance.PlayerHP -= EnemyCardManager.Instance.CardsInGamePower[i];
            RemoveKey(i);
            foreach (var card in EnemyCardManager.Instance.CardsInGame[i])
            {
                card.SetActive(false);
            }
            EnemyCardManager.Instance.RemoveCardFromGame(i);
            EnemyCardManager.Instance.cardHolders[i].GetComponent<CardHolderManager>().ResetCardHolderEnemyCards();
        }

        RoundManager.Instance.PrepareNewRound();
    }

    public void RemoveKey(int key)
    { 
        UIManager.Instance.SetRoundForEnemyUI(key);
        RoundsForCards.Remove(key);
    }
}
