using System.Collections.Generic;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
   public static DamageManager Instance { get; private set; }
   [SerializeField] private RoundManager _roundManager;
   [SerializeField] private EnemyAttackManager _enemyAttackManager;

   [SerializeField] private List<CardHolderManager> _cardHolders;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            //Debug.LogWarning("Another DamageManger found");
        }
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
            if (EnemyCardManager.Instance.CardsInGamePower[i] > 0) continue;

            _roundManager.HealthUpdate(forPlayer: 0, forEnemy: EnemyCardManager.Instance.CardsInGamePower[i]);
            _cardHolders[i].ResetCardHolderEnemyCards();

            foreach (var card in EnemyCardManager.Instance.CardsInGame[i])
            {
                card.SetActive(false);
            }
            EnemyCardManager.Instance.RemoveCardFromGame(key: i);
            _enemyAttackManager.RemoveKey(key: i);
        }

        _enemyAttackManager.UpdateRounds();

    }

}
