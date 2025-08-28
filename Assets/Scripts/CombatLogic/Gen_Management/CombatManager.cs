using MapSystem;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }
    
    //need to be set by the map field before we load combat scene 
    public NodeType CurrentNodeType { get; set; } = NodeType.Boss; // temp default value

    [SerializeField] private List<StructForDictionary<NodeType, List<StructForDictionary<int, List<CardsSOTest>>>>> _listDifferentTypes;

    [SerializeField] private List<StructForDictionary<NodeType, StructForDictionary<int, int>>> _listCombatDetails; 

    private StructForDictionary<int, List<CardsSOTest>> _innerDeck = new();

    private Dictionary<NodeType, Dictionary<int, List<CardsSOTest>>> _decksForDifferentTypes = new();
    private Dictionary<NodeType, StructForDictionary<int, int>> _combatDetails = new();
    private Dictionary<NodeType, int> _deckSize = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;

        else Destroy(gameObject);

        SetProperties();
    }

    private void SetProperties()
    {
        _decksForDifferentTypes.Clear();
        _combatDetails.Clear();
        _deckSize.Clear();

        foreach (var enemyType in _listDifferentTypes)
        {
            _decksForDifferentTypes[enemyType.key] = new Dictionary<int, List<CardsSOTest>>();

            for (int i = 0; i < enemyType.value.Count; i++)
            {
                _innerDeck = enemyType.value[i];
                _decksForDifferentTypes[enemyType.key].Add(_innerDeck.key, new List<CardsSOTest>(_innerDeck.value)); //?
            }
            _deckSize[enemyType.key] = _decksForDifferentTypes[enemyType.key].Keys.Count * _decksForDifferentTypes[enemyType.key][0].Count; //?
        }

        foreach (var detail in _listCombatDetails)
        {
            _combatDetails.Add(detail.key, detail.value);
        }
    }

   public StructForDictionary<int, int> GetCurrentCombatDetails()
    {
        return _combatDetails[CurrentNodeType];
    }

    public int GetCurrentDeckSize()
    {
        return _deckSize[CurrentNodeType];
    }

    public Dictionary<int, List<CardsSOTest>> GetCurrentDeck()
    {
        return _decksForDifferentTypes[CurrentNodeType];
    }   
}
