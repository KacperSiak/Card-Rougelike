using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager : PersistenSingleton<DamageManager>
{
   [SerializeField] private EnemyAttackManager _enemyAttackManager;
   [SerializeField] private PlayerCardsAnimation _playerCardsAnimation;

   [SerializeField] private List<CardHolderLogic> _cardHolders;

    private int _bonusDamage = 0;
    private float _bonusMultiplier = 1;
    private Dictionary<float, float> _bonusMultDamage = new Dictionary<float, float>();

    protected override void OnSingletonAwake() { }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    //here  
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

            foreach (var node in _bonusMultDamage)
            {
                if(CalculateChances(node.Key))
                {
                    _bonusMultiplier *= node.Value;
                }
            }

            RoundManager.Instance.EnemyHP -= Mathf.RoundToInt(-((EnemyCardManager.Instance.CardsInGamePower[i] - _bonusDamage) * _bonusMultiplier)); // cuz number in the list will be negative after we will implement player impact

            _bonusMultiplier = 1;
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

    public void BonusMultiplierChange(float chances, float damage, bool add)
    {
        if (add) _bonusMultDamage[chances] = damage;
        else _bonusMultDamage.Remove(chances);
    }

    private bool CalculateChances(float chances)
    {
        chances = Mathf.Clamp(chances, 0f, 100f);

        float roll = Random.Range(0f, 100f);

        return roll < chances;
    }

    
    
}
