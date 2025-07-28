using System;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    // I think I need to do Global Combat Manager in the nearest future, to take care of some global logic, like cursor, unload the scene, end the combat, but for now I will keep it here

    public static RoundManager Instance { get; private set; }

    [field: SerializeField] public List<CardsSOTest> EnemyCards { get; private set; }
    [field: SerializeField] public List<CardsSOTest> PlayerCards { get; private set; }
    private bool _alreadyFinished = false; // to prevent double calls of the end of combat

    public static event Action OnHPChanged;
    public static event Action OnCombatEnd; // logic is mostly temporary, to mark the end of combat and enable any interactions, but later it will be modified to be built in the whole game logic
    public static event Action OnCombatStart;
    public static event Action OnRoundEnd;

    void Awake()
    {
        if (Instance == null) Instance = this;

        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        OnHPChanged += CheckHPState;
        OnCombatEnd += ReturnHealthValueAfterCombat;
    } 
    
    
    private void OnDisable()
    {
        OnHPChanged -= CheckHPState;
        OnCombatEnd -= ReturnHealthValueAfterCombat;
    }

    private int _playerHP;

    public int PlayerHP
    { 
        get => _playerHP;
        set
        {
            _playerHP = value;
            OnHPChanged?.Invoke();
        }
    }

    private int _enemyHP = 50; // temp default value

    public int EnemyHP
    {
        get => _enemyHP;
        set
        {
            _enemyHP = value;
            OnHPChanged?.Invoke();
        }
    }

    void Start()
    {
        Cursor.visible = true;
        _playerHP = PlayerManager.Instance.PlayerHP;
        OnCombatStart?.Invoke();
    }

    private void ReturnHealthValueAfterCombat()
    { 
        PlayerManager.Instance.PlayerHP = _playerHP;
    }

    public void EndRound()
    {
        DamageManager.Instance.CalculatePlayerImpact();

        if(!_alreadyFinished)
        CheckRoundReadiness();
    }

    private void CheckRoundReadiness()
    {
        if (!PlayerCardManager.Instance.CheckRoundReadiness())
        {
            UIManager.Instance.Winner = "Enemy";
            OnCombatEnd?.Invoke();
            return;
        }
        else if (!EnemyCardManager.Instance.CheckRoundReadiness())
        {
            UIManager.Instance.Winner = "Player";
            OnCombatEnd?.Invoke();
            return;
        }

        MoveToNextRound();

    }

    private void MoveToNextRound()
    { 
        OnRoundEnd?.Invoke();
    }

    private void CheckHPState()
    {
        if(_playerHP > 0 && _enemyHP > 0) return;

        UIManager.Instance.Winner = _playerHP > 0 ? "Player" : "Enemy";
        _alreadyFinished = true; 
        OnCombatEnd?.Invoke();
    }
}
