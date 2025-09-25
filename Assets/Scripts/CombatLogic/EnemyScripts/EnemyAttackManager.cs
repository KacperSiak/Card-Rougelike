using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackManager : MonoBehaviour
{
    public Dictionary<int, int> RoundsForCards { get; private set; } = new(); 
    [SerializeField] private List<DiceLogic> _diceLogic;
    private int _round;
    private const int k_powerIndex = 6; 
    private const int k_minRound = 1;
    private const int k_maxRound = 5;
    private const int k_limit = 3;
    private ConsumableContext _consContext;
    private void OnEnable()
    {
        RoundManager.OnCombatEnd += OnCombatEnd;
        RoundManager.OnCombatStart += StartCombatSetup;
        _consContext = new ConsumableContext();
    }


    private void OnDisable()
    {
        RoundManager.OnCombatEnd -= OnCombatEnd;
        RoundManager.OnCombatStart -= StartCombatSetup;
    }

    public void StartCombatSetup()
    {
        RoundsForCards.Clear();
    }

    public void SetRoundAttack(int key)
    {
        int _cardsPower = EnemyCardManager.Instance.CardsInGamePower[key];

        _round = CalculateRound(_cardsPower);

        RoundsForCards.Add(key, _round);

        _diceLogic[key].UpdateRounds(_round); 

    }

    private int CalculateRound(int totalPower)
    { 
        int amount = Mathf.CeilToInt(totalPower / (float)k_powerIndex); 

        return Mathf.Clamp(amount, k_minRound, k_maxRound);
    }

    public void UpdateRounds()
    {
        StartCoroutine(Animated());
    }

    private IEnumerator Animated()
    {
        for (int i = 0; i < k_limit; i++)
        {
            if (!RoundsForCards.ContainsKey(i))
            {
                _diceLogic[i].UpdateRounds(key: 0);
                continue;
            }

            RoundsForCards[i]--;
            _diceLogic[i].UpdateRounds(RoundsForCards[i]);

            if (RoundsForCards[i] > 0) continue;
         

            //if(_consContext.immune) RoundManager.Instance.PlayerHP -= EnemyCardManager.Instance.CardsInGamePower[i] * 0;
            if (_consContext.vulnerable) RoundManager.Instance.PlayerHP -= EnemyCardManager.Instance.CardsInGamePower[i] * 2;
            else if(!_consContext.immune)
            {
                EnemyCardManager.Instance.Attack(i);
                
                while(EnemyCardManager.Instance.GetCurrentAnimationManager(i).IsAnimating)
                yield return null;

                RoundManager.Instance.PlayerHP -= EnemyCardManager.Instance.CardsInGamePower[i]; 
                UIManager.Instance.ShowPlayerDamage(EnemyCardManager.Instance.CardsInGamePower[i]);
            }

            if (RoundManager.Instance.PlayerHP <= 0) break;

            RemoveKey(i);
            EnemyCardManager.Instance.RemoveCardFromGame(i, EnemyCardManager.Instance.CardsInGame[i].Count);
            EnemyCardManager.Instance.cardHolders[i].GetComponent<CardHolderLogic>().ResetCardHolderEnemyCards();
        }

        RoundManager.Instance.CheckRoundReadiness();
    }

    public void RemoveKey(int key)
    { 
        RoundsForCards.Remove(key);
    }

    private void OnCombatEnd()
    { 
        RoundsForCards.Clear();
    }
}
