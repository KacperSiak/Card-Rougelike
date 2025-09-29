using MapSystem;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }
    
    public static NodeType NextCombatNodeType = NodeType.Enemy;
    public static MapSystem.EnemyVariant NextEnemyVariant = MapSystem.EnemyVariant.None;
    public static MapSystem.EliteVariant NextEliteVariant = MapSystem.EliteVariant.None;
    
    //need to be set by the map field before we load combat scene 
    public NodeType CurrentNodeType { get; set; } = NodeType.Boss; // temp default value
    public MapSystem.EnemyVariant CurrentEnemyVariant { get; set; } = MapSystem.EnemyVariant.None;
    public MapSystem.EliteVariant CurrentEliteVariant { get; set; } = MapSystem.EliteVariant.None;

    [SerializeField] private List<StructForDictionary<NodeType, List<StructForDictionary<int, List<CardsSO>>>>> _listDifferentTypes;

    [SerializeField] private List<StructForDictionary<NodeType, StructForDictionary<int, int>>> _listCombatDetails;

    [SerializeField] private List<StructForDictionary<int, StructForDictionary<int, int>>> diceInfo;

    private StructForDictionary<int, List<CardsSO>> _innerDeck = new();

    private Dictionary<NodeType, Dictionary<int, List<CardsSO>>> _decksForDifferentTypes = new();
    private Dictionary<NodeType, StructForDictionary<int, int>> _combatDetails = new();
    private Dictionary<NodeType, int> _deckSize = new();
    public Dictionary<int, StructForDictionary<int, int>> _diceDictionary { get; private set; } = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

    CurrentNodeType = NextCombatNodeType;
    CurrentEnemyVariant = NextEnemyVariant;
    CurrentEliteVariant = NextEliteVariant;
        
        SetProperties();
    }

    private void OnEnable()
    {
        CurrentNodeType = NextCombatNodeType;
    CurrentEnemyVariant = NextEnemyVariant;
    CurrentEliteVariant = NextEliteVariant;
    }

    public static void ApplyNodeType(NodeType type)
    {
        NextCombatNodeType = type;
        if (Instance != null)
        {
            Instance.CurrentNodeType = type;
        }
    }

    public static void ApplyEnemyVariant(MapSystem.EnemyVariant variant)
    {
        NextEnemyVariant = variant;
        if (Instance != null)
        {
            Instance.CurrentEnemyVariant = variant;
        }
    }

    public static void ApplyEliteVariant(MapSystem.EliteVariant variant)
    {
        NextEliteVariant = variant;
        if (Instance != null)
        {
            Instance.CurrentEliteVariant = variant;
        }
    }

    private void OnGUI()
    {
        var text = $"Enemy Type: {GetCurrentEnemyTypeName()}";
        var style = new GUIStyle(GUI.skin.label)
        {
            fontSize = 18,
            normal = { textColor = Color.white }
        };
        GUI.Label(new Rect(10, Screen.height - 30, 320, 28), text, style);
    }

    private string GetCurrentEnemyTypeName()
    {
        if (CurrentNodeType == NodeType.Enemy)
        {
            return CurrentEnemyVariant == MapSystem.EnemyVariant.Satyr ? "Satyr" : "Griffon";
        }
        if (CurrentNodeType == NodeType.Elite)
        {
            return CurrentEliteVariant == MapSystem.EliteVariant.Arachne ? "Arachne" : "Minotaur";
        }
        switch (CurrentNodeType)
        {
            case NodeType.Boss: return "Boss";
            default: return CurrentNodeType.ToString();
        }
    }


    private void SetProperties()
    {
        _decksForDifferentTypes.Clear();
        _combatDetails.Clear();
        _deckSize.Clear();
        _diceDictionary.Clear();

        foreach (var enemyType in _listDifferentTypes)
        {
            _decksForDifferentTypes[enemyType.key] = new Dictionary<int, List<CardsSO>>();

            for (int i = 0; i < enemyType.value.Count; i++)
            {
                _innerDeck = enemyType.value[i];
                _decksForDifferentTypes[enemyType.key].Add(_innerDeck.key, new List<CardsSO>(_innerDeck.value)); //?
            }
            _deckSize[enemyType.key] = _decksForDifferentTypes[enemyType.key].Keys.Count * _decksForDifferentTypes[enemyType.key][0].Count; //?
        }

        foreach (var detail in _listCombatDetails)
        {
            _combatDetails.Add(detail.key, detail.value);
        }

        foreach (var dice in diceInfo)
        {
            _diceDictionary.Add(dice.key, dice.value);
        }
    }

   public StructForDictionary<int, int> GetCurrentCombatDetails()
    {
    if (_combatDetails.TryGetValue(CurrentNodeType, out var details)) return details;
    Debug.LogWarning($"[CombatManager] Missing combat details for {CurrentNodeType}. Using Enemy.");
    return _combatDetails.ContainsKey(NodeType.Enemy) ? _combatDetails[NodeType.Enemy] : new StructForDictionary<int, int>();
    }

    public int GetCurrentDeckSize()
    {
    if (_deckSize.TryGetValue(CurrentNodeType, out var size)) return size;
    Debug.LogWarning($"[CombatManager] Missing deck size for {CurrentNodeType}. Using Enemy size if available.");
    return _deckSize.ContainsKey(NodeType.Enemy) ? _deckSize[NodeType.Enemy] : 0;
    }

    public Dictionary<int, List<CardsSO>> GetCurrentDeck()
    {
    if (_decksForDifferentTypes.TryGetValue(CurrentNodeType, out var deck)) return deck;
    Debug.LogWarning($"[CombatManager] Missing deck for {CurrentNodeType}. Using Enemy deck if available.");
    return _decksForDifferentTypes.ContainsKey(NodeType.Enemy) ? _decksForDifferentTypes[NodeType.Enemy] : new Dictionary<int, List<CardsSO>>();
    }
}
