using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [SerializeField] private PlayerAttackManager _playerAttackManager;
    [SerializeField] private EnemyAttackManager _enemyAttackManager;
    [SerializeField] private List<CardsSOTest> _enemyCards;
    [SerializeField] private List<CardsSOTest> _playerCards;

    void Start()
    {
        Cursor.visible = true;
        _playerAttackManager.StartCombatSetup();
        _enemyAttackManager.StartCombatSetup();
        PlayerCardManager.Instance.StartCombat(_playerCards);
        EnemyCardManager.Instance.StartCombat(_enemyCards);
    }
}
