using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    // I think I need to do Global Combat Manager in the nearest future, to take care of some global logic, like cursor, unload the scene, end the combat, but for now I will keep it here

    public static RoundManager Instance { get; private set; }

    [SerializeField] private EnemyAttackManager _enemyAttackManager;
    [SerializeField] private List<CardsSOTest> _enemyCards;
    [SerializeField] private List<CardsSOTest> _playerCards;

    public static event Action OnCombatEnd;
    //public static event Action OnRoundEnd;

    // For the end of the combat there can be an event and many components will have methods to react on this event
    // Maybe for more stages I can create events, like OnCombatStart, OnCombatEnd, OnRoundStart, OnRoundEnd, etc.

    void Awake()
    {
        if (Instance == null) Instance = this;

        else Destroy(gameObject);
    }

    private int _playerHP = 50; // temp default value

    public int PlayerHP
    { 
        get => _playerHP;
        set
        {
            _playerHP = value;
            UIManager.Instance.SetHealthUI();
        }
    }

    private int _enemyHP = 50; // temp default value

    public int EnemyHP
    {
        get => _enemyHP;
        set
        {
            _enemyHP = value;
            UIManager.Instance.SetHealthUI();
        }
    }

    void Start()
    {
        Cursor.visible = true; 
        UIManager.Instance.SetHealthUI();
        _enemyAttackManager.StartCombatSetup();
        PlayerCardManager.Instance.StartCombat(_playerCards);
        EnemyCardManager.Instance.StartCombat(_enemyCards);
    }

    public void EndRound()
    {
        DamageManager.Instance.CalculatePlayerImpact();
    }

    public void CheckCurrentCombatState()
    { 
    
    }

    public void PrepareNewRound()
    { 
        PlayerCardManager.Instance.RefillCards();
        EnemyCardManager.Instance.RefillCards();
    }

    private void EndCombat()
    { 
    
    }
}
