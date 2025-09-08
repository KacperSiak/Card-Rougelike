using System.Collections.Generic;
using UnityEngine;

public class DamageManager : PersistenSingleton<DamageManager>
{
   [SerializeField] private EnemyAttackManager _enemyAttackManager;

   [SerializeField] private List<CardHolderLogic> _cardHolders;

    public void CalculatePlayerImpact()
    {
        foreach (var ch in _cardHolders)
        {
            if (ch.ValidatedPlayerCardsPower == 0) continue;

            //ch.CardHolderAnimation.DisableAnimations();
            //ch.Attack();
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
