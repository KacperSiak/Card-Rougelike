using System.Collections.Generic;
using TMPro;
using UnityEditor.TextCore.Text;
using UnityEngine;
using UnityEngine.Events;

public class RoundManager : MonoBehaviour
{
    [SerializeField] private EnemyAttackManager _enemyAttackManager;
    [SerializeField] private List<CardsSOTest> _enemyCards;
    [SerializeField] private List<CardsSOTest> _playerCards;
    [SerializeField] private TextMeshProUGUI _playerHPText;
    [SerializeField] private TextMeshProUGUI _enemyHPText;

    //obsenrever pattern
	[SerializeField] private int _playerHP = 50;
	public int PlayerHP 
    { 
        get => _playerHP;
        set
        {
            _playerHP = value;
            OnPlayerHPChange.Invoke();
		}
    } // temp default value
	//[field: SerializeField] public int PlayerHP { get; private set; } = 50; // temp default value
    public int EnemyHP { get; private set; } = 50; // temp default values

    public UnityEvent OnPlayerHPChange { get; private set; } = new();
    public UnityEvent OnRoundEnd { get; private set; } = new();
    public UnityEvent OnRoundStart { get; private set; } = new();
    public UnityEvent OnSomething { get; private set; } = new();

    void Start()
    {
        Cursor.visible = true; //seperate manager
        HealthUI();
        _enemyAttackManager.StartCombatSetup();
        PlayerCardManager.Instance.StartCombat(_playerCards);
        EnemyCardManager.Instance.StartCombat(_enemyCards);
    }

    //public void UpdateHealthUI()
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
		//PrepareNewRound();
		//OnRoundEnd.Invoke();
	}

    public void PrepareNewRound()
    { 
        PlayerCardManager.Instance.RefillCards();
        EnemyCardManager.Instance.RefillCards();
    }
}
