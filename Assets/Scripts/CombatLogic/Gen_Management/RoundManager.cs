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
    private bool _deathRegenStarted = false; // guard for map regeneration on death

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
            _ = SceneLoader.Instance.manager.ActivateScene("MapGeneration");
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
    if(_alreadyFinished) return;
    if(_playerHP > 0 && _enemyHP > 0) return;

    Debug.Log($"[RoundManager] Combat end trigger: playerHP={_playerHP} enemyHP={_enemyHP} winner={( _playerHP>0 ? "Player" : "Enemy")}");
    UIManager.Instance.Winner = _playerHP > 0 ? "Player" : "Enemy";
    // Nie generuj tu od razu – scena mapy może jeszcze nie być aktywowana.
    if (_playerHP <= 0)
    {
        StartDeathRegenDeferred(); // odpalimy po aktywacji sceny / gdy generator będzie dostępny
    }
    _alreadyFinished = true; 
    OnCombatEnd?.Invoke();
    }

    private void LogCombatResult()
    {
        Debug.Log($"[RoundManager][Result] Winner={UIManager.Instance.Winner} PlayerHP={_playerHP} EnemyHP={_enemyHP}");
    }

    private void TryRegenerateMapOnPlayerDeath()
    {
        var gen = FindActiveOrInactiveMapGenerator();
        if (gen == null)
        {
            Debug.Log("[RoundManager] MapGenerator still not present; will keep waiting...");
            return; // coroutine caller will retry
        }
        var saveMgr = FindActiveOrInactive<MapSaveManager>();
        if (saveMgr != null)
        {
            saveMgr.DeleteSavedMap();
        }
        gen.fixedSeed = 0; // losowy seed
        gen.GenerateMap();
        Debug.Log("[RoundManager] Player death -> map regenerated (final)");
        _deathRegenStarted = false; // allow future deaths
    }

    private void StartDeathRegenDeferred()
    {
        if (_deathRegenStarted) return;
        _deathRegenStarted = true;
        StartCoroutine(DeathRegenRoutine());
    }

    private IEnumerator DeathRegenRoutine()
    {
        Debug.Log("[RoundManager] DeathRegenRoutine started (waiting for MapGenerator)");
        // czekaj kilka klatek na aktywację sceny mapy
        for (int frame = 0; frame < 60; frame++)
        {
            var gen = FindActiveOrInactiveMapGenerator();
            if (gen != null)
            {
                TryRegenerateMapOnPlayerDeath();
                yield break;
            }
            yield return null;
        }
        Debug.LogWarning("[RoundManager] DeathRegenRoutine timeout (MapGenerator not found in 60 frames)");
        _deathRegenStarted = false;
    }

    private MapGenerator FindActiveOrInactiveMapGenerator()
    {
        var gen = FindObjectOfType<MapGenerator>();
        if (gen != null) return gen;
        // Spróbuj także w nieaktywnych (Resources.FindObjectsOfTypeAll)
        foreach (var g in Resources.FindObjectsOfTypeAll<MapGenerator>())
        {
            if (g.hideFlags == HideFlags.None)
                return g;
        }
        return null;
    }

    private T FindActiveOrInactive<T>() where T : UnityEngine.Object
    {
        var obj = FindObjectOfType<T>();
        if (obj != null) return obj;
        foreach (var o in Resources.FindObjectsOfTypeAll<T>())
        {
            if (o.hideFlags == HideFlags.None)
                return o;
        }
        return null;
    }

}