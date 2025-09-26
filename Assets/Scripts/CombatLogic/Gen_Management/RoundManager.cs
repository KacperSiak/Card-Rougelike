using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Systems.SceneManagement;

public class RoundManager : PersistenSingleton<RoundManager>
{
    // I think I need to do Global Combat Manager in the nearest future, to take care of some global logic, like cursor, unload the scene, end the combat, but for now I will keep it here

    [field: SerializeField] public List<CardsSOTest> PlayerCards { get; private set; }
    [SerializeField] private AthenaAnimationLogic _athenaAnimationLogic;
    [SerializeField] private TrumpSuitSetup _trumpSuitSetup;
    public StructForDictionary<int, int> CurrentCombatDetails { get; private set; } 
    public Suits TrumpSuit { get; private set; } 
    private bool _alreadyFinished = false; // to prevent double calls of the end of combat

    public bool damageBlock; // Kacper Added

    public static event Action OnHPChanged;
    public static event Action OnCombatEnd; 
    public static event Action OnCombatStart;
    public static event Action OnRoundEnd;

    //private ConsumableContext _context;

    private void OnEnable()
    {
        OnHPChanged += CheckHPState;
        OnCombatEnd += ReturnHealthValueAfterCombat;
        OnCombatEnd += SwitchToMainScene;
        OnCombatEnd += LogCombatResult;
        OnCombatEnd += HandleCombatEndMarking;
        ConsumablesEventSystem.OnInstaWin.AddListener(CombatEndCard);
    } 
    
    
    private void OnDisable()
    {
        OnHPChanged -= CheckHPState;
        OnCombatEnd -= ReturnHealthValueAfterCombat;
        OnCombatEnd -= SwitchToMainScene;
        OnCombatEnd -= LogCombatResult;
        OnCombatEnd -= HandleCombatEndMarking;
        ConsumablesEventSystem.OnInstaWin.RemoveListener(CombatEndCard);
    }

    private int _playerHP;

    public int PlayerHP
    { 
        get => _playerHP;
        set
        {
            if (damageBlock) return; // Kacper Added

            _playerHP = value;
            PlayerManager.Instance.PlayerHP = _playerHP; 
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
        _alreadyFinished = false;
        Debug.Log("Combat started.");
        Cursor.visible = true;
        _playerHP = PlayerManager.Instance.PlayerHP;
        CurrentCombatDetails = CombatManager.Instance.GetCurrentCombatDetails();
        StartCoroutine(CheckCombatReadiness());
        TrumpSuit = _trumpSuitSetup.ChooseSuit();
        Debug.Log("Trump suit: " + TrumpSuit);
    }

    private IEnumerator CheckCombatReadiness()
    {
        while (!DeckAnimation.Instance.IsDeckReady)
            yield return null;

        OnCombatStart?.Invoke();
        _trumpSuitSetup.SetUpTrumpCard((int)TrumpSuit);
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
        Debug.Log($"Combat ended. Winner: {UIManager.Instance.Winner}");
        if (SceneLoader.Instance != null)
        {
            StartCoroutine(SwitchToMainSceneFlow());
        }
        else
        {
            bool playerWon = UIManager.Instance.Winner == "Player";
            if (playerWon)
            {
                Debug.Log("Win — returning to map. Node cleared.");
                NodeClickHandler.MarkLastCombatNodeClearedIfAny();
            }
            else
            {
                Debug.Log("Defeat — resetting map and reloading.");
                NodeClickHandler.ClearClearedNodes();
            }
            UnityEngine.SceneManagement.SceneManager.LoadScene("MapGeneration");
        }
    }

    private IEnumerator SwitchToMainSceneFlow()
    {
        bool playerWon = UIManager.Instance.Winner == "Player";
        Debug.Log($"Switching to map. Result: {(playerWon ? "win" : "defeat")}.");

        if (playerWon)
        {
            var task = SceneLoader.Instance.manager.ActivateScene("MapGeneration");
            while (task != null && !task.IsCompleted)
                yield return null;
            Debug.Log("Win — back to map (no reload).");
        }
        else
        {
            Debug.Log("Defeat — reloading map.");
            GlobalRunState.FlagDefeatReload();
            yield return StartCoroutine(ReloadMapSceneCoroutine());
        }
    }

    private IEnumerator ReloadMapSceneCoroutine()
    {
        Debug.Log("Clearing map state...");
        NodeClickHandler.ClearClearedNodes();
        NodeClickHandler.currentPlayerNode = null;
        NodeClickHandler.currentPlayerMapNode = null;
        if (NodeClickHandler.playerMarker != null)
        {
            NodeClickHandler.playerMarker.SetActive(false);
        }

        var saveMgr = GameObject.FindObjectOfType<MapSaveManager>(includeInactive: true);
        if (saveMgr != null)
        {
            saveMgr.DeleteSavedMap();
            Debug.Log("Deleting saved map.");
        }

        var mapScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName("MapGeneration");
        if (mapScene.IsValid() && mapScene.isLoaded)
        {
            Debug.Log("Unloading map scene...");
            var unloadOp = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(mapScene);
            while (unloadOp != null && !unloadOp.isDone)
                yield return null;
            yield return Resources.UnloadUnusedAssets();
        }
        else
        {
            Debug.Log("Map scene not loaded.");
        }

        if (SceneLoader.Instance != null)
        {
            Debug.Log("Switching to map scene...");
            var task = SceneLoader.Instance.manager.ActivateScene("MapGeneration");
            while (task != null && !task.IsCompleted)
                yield return null;
            Debug.Log("Map ready.");
        }
        else
        {
            Debug.LogWarning("SceneLoader not found — loading map directly.");
            var loadOp = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MapGeneration", UnityEngine.SceneManagement.LoadSceneMode.Additive);
            while (loadOp != null && !loadOp.isDone)
                yield return null;
            var loaded = UnityEngine.SceneManagement.SceneManager.GetSceneByName("MapGeneration"); // zmienic do zmiennej "MapGeneration"
            if (loaded.IsValid())
                UnityEngine.SceneManagement.SceneManager.SetActiveScene(loaded);
        }

        yield return null;
        GlobalRunState.ClearReloadFlags(); // flagi mozna usunac
    }

    public void EndRound()
    {
        _athenaAnimationLogic.RoundFinishAnimation();
        DamageManager.Instance.CalculatePlayerImpact();

        //if(!_alreadyFinished) //here
        //CheckRoundReadiness();
    }

    //temp public 
    public void CheckRoundReadiness()
    {
        if (_alreadyFinished) return;

        if (!PlayerCardManager.Instance.CheckRoundReadiness())
        {
            UIManager.Instance.Winner = "Enemy"; // mozna zmienic na bool
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
        //if(!_context.bonusRound) MoveToNextRound();
        //else _context.bonusRound = false;

    }

    private void MoveToNextRound()
    { 
        OnRoundEnd?.Invoke();
    }

    private void CheckHPState()
    {

        if (_playerHP > 0 && _enemyHP > 0) return;

        Debug.Log($"[RoundManager] Combat end trigger: playerHP={_playerHP} enemyHP={_enemyHP} winner={( _playerHP>0 ? "Player" : "Enemy")}");
        UIManager.Instance.Winner = _playerHP > 0 ? "Player" : "Enemy";
        _alreadyFinished = true;
        OnCombatEnd?.Invoke();
    }

    private void CombatEndCard()
    {
        if (_alreadyFinished) return;
        UIManager.Instance.Winner = "Player";
        _alreadyFinished = true;
        Debug.Log("[RoundManager] Insta-win: setting Winner=Player and ending combat");
        OnCombatEnd?.Invoke();
    }

    private void HandleCombatEndMarking()
    {
        bool playerWon = false;
        var winner = UIManager.Instance.Winner;
        if (!string.IsNullOrEmpty(winner))
        {
            playerWon = string.Equals(winner, "Player", StringComparison.OrdinalIgnoreCase);
        }
        if (!playerWon && _playerHP > 0 && _enemyHP <= 0)
        {
            Debug.Log("[RoundManager] Winner fallback via HP check -> Player");
            playerWon = true;
        }

        if (playerWon)
        {
            Debug.Log("[RoundManager] OnCombatEnd: Player won. Marking last combat node as cleared now.");
            NodeClickHandler.MarkLastCombatNodeClearedIfAny();
        }
        else
        {
            Debug.Log("[RoundManager] OnCombatEnd: Player did not win. No marking performed.");
        }
    }

    private void LogCombatResult()
    {
        Debug.Log($"[RoundManager][Result] Winner={UIManager.Instance.Winner} PlayerHP={_playerHP} EnemyHP={_enemyHP}");
    }
}