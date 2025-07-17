using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyAttackManager : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> _rounds;
    [SerializeField] public Dictionary<int, int> roundsForCards { get; private set; }
    private int _round;

    void Start()
    {
        roundsForCards = new Dictionary<int, int>();
    }

    public void SetRoundAttack(int key)
    {
        int _cardsPower = 0;

        foreach (var card in EnemyCardManager.Instance.CardsInGame[key])
        {
            _cardsPower += card.GetComponent<EnemyCardLogic>().Card.power;
        }

        _round = CalculateRound(_cardsPower);

        _rounds[key].text = $"{_round}";

        roundsForCards.Add(key, _round);
    }

    private int CalculateRound(int totalPower)
    { 
        int amount = Mathf.CeilToInt(5f * (totalPower - 2f) / (42f - 2f));

        return Mathf.Clamp(amount, 1, 5);
    }
}
