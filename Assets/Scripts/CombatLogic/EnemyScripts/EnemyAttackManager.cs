using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyAttackManager : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> _rounds;
    [SerializeField] private RoundManager _roundManager;
    [SerializeField] public Dictionary<int, int> RoundsForCards { get; private set; } //update = new();
    private int _round;

    //private const float k_rate = 8f;

    public void StartCombatSetup()
    {
        RoundsForCards = new Dictionary<int, int>();
        //RoundsForCards.Clear();
	}

    public void SetRoundAttack(int key)
    {
        int _cardsPower = EnemyCardManager.Instance.CardsInGamePower[key];

        _round = CalculateRound(_cardsPower);

        _rounds[key].text = $"{_round}";

        RoundsForCards.Add(key, _round);
    }

    private int CalculateRound(int totalPower)
    { 
        int amount = Mathf.CeilToInt(5f * (totalPower - 2f) / (42f - 2f));
		//int amount = Mathf.CeilToInt((totalPower - 2f) / k_rate);

		return Mathf.Clamp(amount, 1, 5); //magic numbers
    }

    public void UpdateRounds()
    {
        int limit = _rounds.Count;

        for (int i = 0; i < limit; i++)
        {
            if (!RoundsForCards.ContainsKey(i)) continue;

            RoundsForCards[i]--;
            if (RoundsForCards[i] > 0)
            {
                UpdateUI(key: i);
                continue;
            } 

            _roundManager.HealthUpdate(forPlayer: EnemyCardManager.Instance.CardsInGamePower[i], forEnemy: 0);
            //_roundManager.PlayerHP -= EnemyCardManager.Instance.CardsInGamePower[i];

			RemoveKey(i);
            foreach (var card in EnemyCardManager.Instance.CardsInGame[i])
            {
                card.SetActive(false);
            }
            EnemyCardManager.Instance.RemoveCardFromGame(i);
            EnemyCardManager.Instance.cardHolders[i].GetComponent<CardHolderManager>().ResetCardHolderEnemyCards();
        }

		_roundManager.PrepareNewRound(); //move to round manager
	}

    private void UpdateUI(int key)
    {
        _rounds[key].text = $"{RoundsForCards[key]}";
    }

    public void RemoveKey(int key)
    { 
        RoundsForCards.Remove(key);
        _rounds[key].text = string.Empty;
    }
}
