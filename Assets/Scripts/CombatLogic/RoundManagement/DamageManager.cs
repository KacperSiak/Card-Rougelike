using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager : PersistenSingleton<DamageManager>
{
   [SerializeField] private EnemyAttackManager _enemyAttackManager;
   [SerializeField] private PlayerCardsAnimation _playerCardsAnimation;

   [SerializeField] private List<CardHolderLogic> _cardHolders;

    private int _bonusDamage = 0;
    private float multiply = 1;
    private ConsumableContext _consContext;

    protected override void OnSingletonAwake() 
    {
        _consContext = new ConsumableContext();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    public void CalculatePlayerImpact()
    {
        StartCoroutine(Animated());
    }

    private IEnumerator Animated()
    { 
       foreach (var ch in _cardHolders)
       {
           if (ch.ValidatedPlayerCardsPower == 0) continue;

           ch.CardHolderAnimation.DisableAnimations();
           ch.Attack();

            while(_playerCardsAnimation.IsAnimating)
                yield return null;

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
            int dmg = (int)Mathf.RoundToInt(-((EnemyCardManager.Instance.CardsInGamePower[i] - _bonusDamage) * multiply)); // cuz number in the list will be negative after we will implement player impact
            RoundManager.Instance.EnemyHP -= dmg;

            if (_consContext.selfHit)
            {
                int health = (int)System.Math.Round(dmg * 0.1f, MidpointRounding.AwayFromZero);
                RoundManager.Instance.PlayerHP -= health;
            }
            if (RoundManager.Instance.EnemyHP <= 0) return;

            _cardHolders[i].ResetCardHolderEnemyCards();

            EnemyCardManager.Instance.RemoveCardFromGame(key: i, EnemyCardManager.Instance.CardsInGame[i].Count);
            _enemyAttackManager.RemoveKey(key: i);
        }

        _enemyAttackManager.UpdateRounds();
    }

    public void BonusDamageChange(int amount, bool add)
    {
        if (add) _bonusDamage += amount;
        else _bonusDamage -= amount;
    }

    public void MultiplyDamage(float amount, bool add)
    {
        if (add) multiply *= amount;
        else multiply /= amount;
    }
}
