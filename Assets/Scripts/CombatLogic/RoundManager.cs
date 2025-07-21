using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [SerializeField] private EnemyAttackManager _enemyAttackManager;
    [SerializeField] private List<CardsSOTest> _enemyCards;
    [SerializeField] private List<CardsSOTest> _playerCards;
    [SerializeField] private TextMeshProUGUI _playerHPText;
    [SerializeField] private TextMeshProUGUI _enemyHPText;

    public int PlayerHP { get; private set; } = 50; // temp default value
    public int EnemyHP { get; private set; } = 50; // temp default values

    void Start()
    {
        Cursor.visible = true;
        HealthUI();
        _enemyAttackManager.StartCombatSetup();
        PlayerCardManager.Instance.StartCombat(_playerCards);
        EnemyCardManager.Instance.StartCombat(_enemyCards);
    }

    public void HealthUI()
    { 
        _playerHPText.text = $"Player HP: {PlayerHP}";
        _enemyHPText.text = $"Enemy HP: {EnemyHP}";
    }

    public void HealthUpdate(int forPlayer, int forEnemy)
    {
        PlayerHP -= forPlayer;
        EnemyHP += forEnemy;

        HealthUI();
    }

    public void EndRound()
    {
        DamageManager.Instance.CalculatePlayerImpact();
    }

    public void PrepareNewRound()
    { 
        PlayerCardManager.Instance.RefillCards();
        EnemyCardManager.Instance.RefillCards();
    }
}
