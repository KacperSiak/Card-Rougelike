using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.EventSystems.EventTrigger;
using Systems.SceneManagement;

public class RoundManager : PersistenSingleton<RoundManager>
{
    // I think I need to do Global Combat Manager in the nearest future, to take care of some global logic, like cursor, unload the scene, end the combat, but for now I will keep it here

    [field: SerializeField] public List<CardsSOTest> PlayerCards { get; private set; }
    [SerializeField] private AthenaAnimationLogic _athenaAnimationLogic; 
    public StructForDictionary<int, int> CurrentCombatDetails { get; private set; } 
    public Suits TrumpSuit { get; private set; } 
    private bool _alreadyFinished = false; // to prevent double calls of the end of combat

    public bool damageBlock; // Kacper Added

    public static event Action OnHPChanged;
    public static event Action OnCombatEnd; 
    public static event Action OnCombatStart;
    public static event Action OnRoundEnd;

    private void OnEnable()
    {
        OnHPChanged += CheckHPState;
        OnCombatEnd += ReturnHealthValueAfterCombat;
        OnCombatEnd += SwitchToMainScene;
    OnCombatEnd += LogCombatResult;
    PlayerManager.OnHpChange.AddListener(SyncFromPlayerManager); // keep local copy in sync
    } 
    
    
    private void OnDisable()
    {
        OnHPChanged -= CheckHPState;
        OnCombatEnd -= ReturnHealthValueAfterCombat;
        OnCombatEnd -= SwitchToMainScene;
    OnCombatEnd -= LogCombatResult;
    PlayerManager.OnHpChange.RemoveListener(SyncFromPlayerManager);
    }

    private int _playerHP;

    public int PlayerHP
    { 
        get => _playerHP;
        set
        {
            if (damageBlock) return; // Kacper Added

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
        CurrentCombatDetails = CombatManager.Instance.GetCurrentCombatDetails();
        StartCoroutine(CheckCombatReadiness());
        TrumpSuit = (Suits)UnityEngine.Random.Range(0, Enum.GetValues(typeof(Suits)).Length - 1);
        Debug.Log("Trump suit is: " + TrumpSuit);
    }

    private IEnumerator CheckCombatReadiness()
    {
        while (!DeckAnimation.Instance.IsDeckReady)
            yield return null;

        OnCombatStart?.Invoke();
    }

    private void ReturnHealthValueAfterCombat()
    { 
        PlayerManager.Instance.PlayerHP = _playerHP;
    }

    private void SyncFromPlayerManager()
    {
        var newHp = PlayerManager.Instance.PlayerHP;
        if (newHp != _playerHP)
        {
            _playerHP = newHp;
            Debug.Log($"[RoundManager] SyncFromPlayerManager new HP={_playerHP}");
            OnHPChanged?.Invoke();
        }
    }

    private void SwitchToMainScene()
    {
        if (SceneLoader.Instance != null)
        {
            StartCoroutine(SwitchToMainSceneFlow());
    }
    }

    private System.Collections.IEnumerator SwitchToMainSceneFlow()
    {
        bool playerWon = UIManager.Instance.Winner == "Player";

        var task = SceneLoader.Instance.manager.ActivateScene("MapGeneration");
        while (task != null && !task.IsCompleted)
            yield return null;

        if (!playerWon)
        {
            // Player lost: reset the map to a fresh one (seed=0 => random)
            var saveMgr = GameObject.FindObjectOfType<MapSaveManager>(includeInactive: true);
            if (saveMgr != null)
            {
                saveMgr.DeleteSavedMap();
            }

            var gen = GameObject.FindObjectOfType<MapGenerator>(includeInactive: true);
            if (gen != null)
            {
                // Clear player marker/current node to avoid dependencies on current position
                if (NodeClickHandler.playerMarker != null)
                {
                    NodeClickHandler.playerMarker.SetActive(false);
                }
                NodeClickHandler.currentPlayerNode = null;
                NodeClickHandler.currentPlayerMapNode = null;
                NodeClickHandler.ClearClearedNodes();

                gen.fixedSeed = 0;
                gen.GenerateMap();
                if (saveMgr != null)
                {
                    saveMgr.SaveCurrentMap();
                }
            }
        }
        else
        {
            // Player won: mark the last entered combat node as cleared so revisiting doesn't load scene
            NodeClickHandler.MarkLastCombatNodeClearedIfAny();
        }
    }

    public void EndRound()
    {
        _athenaAnimationLogic.RoundFinishAnimation();
        DamageManager.Instance.CalculatePlayerImpact();

        if(!_alreadyFinished)
        CheckRoundReadiness();
    }

    private void CheckRoundReadiness()
    {
        if (!PlayerCardManager.Instance.CheckRoundReadiness())
        {
            UIManager.Instance.Winner = "Enemy";
            _alreadyFinished = true;
            OnCombatEnd?.Invoke();
            return;
        }
        else if (!EnemyCardManager.Instance.CheckRoundReadiness())
        {
            UIManager.Instance.Winner = "Player";
            _alreadyFinished = true;
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
    if(_alreadyFinished) return;
    if(_playerHP > 0 && _enemyHP > 0) return;

    Debug.Log($"[RoundManager] Combat end trigger: playerHP={_playerHP} enemyHP={_enemyHP} winner={( _playerHP>0 ? "Player" : "Enemy")}");
    UIManager.Instance.Winner = _playerHP > 0 ? "Player" : "Enemy";
    _alreadyFinished = true; 
    OnCombatEnd?.Invoke();
    }

    private void LogCombatResult()
    {
        Debug.Log($"[RoundManager][Result] Winner={UIManager.Instance.Winner} PlayerHP={_playerHP} EnemyHP={_enemyHP}");
    }
}