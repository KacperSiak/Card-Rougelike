using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyAttackManager : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> _rounds;
    [SerializeField] public Dictionary<int, int> RoundsForCards { get; private set; }
    private int _round;

    public void StartCombatSetup()
    {
        RoundsForCards = new Dictionary<int, int>();
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

        return Mathf.Clamp(amount, 1, 5);
    }
}
