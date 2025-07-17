using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [SerializeField] private PlayerCardManager _playerCardManager;
    [SerializeField] private EnemyCardManager _enemyCardManager;
    [SerializeField] private List<CardsSOTest> _enemyCards;
    [SerializeField] private List<CardsSOTest> _playerCards;

    void Start()
    {
        Cursor.visible = true;
        _playerCardManager.StartCombat(_playerCards);
        _enemyCardManager.StartCombat(_enemyCards);
    }
}
