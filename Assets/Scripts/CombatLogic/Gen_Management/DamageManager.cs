using System.Collections.Generic;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
   public static DamageManager Instance { get; private set; }
   [SerializeField] private EnemyAttackManager _enemyAttackManager;

   [SerializeField] private List<CardHolderLogic> _cardHolders;

    void Awake()
    { 
        if (Instance == null) Instance = this;

        else Destroy(gameObject);

    }

    public void CalculatePlayerImpact()
    {
        foreach (var ch in _cardHolders)
        {
            if (ch.ValidatedPlayerCardsPower == 0) continue;

            EnemyCardManager.Instance.UpdatePower(ch.EnemyComboIndex, ch.ValidatedPlayerCardsPower);
            ch.ResetCardHolderPlayerCards();
        }

        ImplementPlayerImpact();
    }

    private void ImplementPlayerImpact()
    {
        for (int i = 0; i < _cardHolders.Count; i++)
        { 
            if (!EnemyCardManager.Instance.CardsInGamePower.ContainsKey(i) || EnemyCardManager.Instance.CardsInGamePower[i] > 0) continue; 

            RoundManager.Instance.EnemyHP -= -EnemyCardManager.Instance.CardsInGamePower[i]; // cuz number in the list will be negative after we will implement player impact

            if (RoundManager.Instance.EnemyHP <= 0) return;

            _cardHolders[i].ResetCardHolderEnemyCards();

            EnemyCardManager.Instance.RemoveCardFromGame(key: i, EnemyCardManager.Instance.CardsInGame[i].Count);
            _enemyAttackManager.RemoveKey(key: i);
        }

        _enemyAttackManager.UpdateRounds();
    }
}
