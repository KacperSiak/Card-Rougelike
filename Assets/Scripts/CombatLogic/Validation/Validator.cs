using UnityEngine;

//jocker validation to be improved 
public class Validator : MonoBehaviour
{
    [SerializeField] protected EnemyAttackManager _enemyAttackManager;
    protected bool _isValid = false;
    protected CardHolderLogic _cardHolder;

    public void Validate(CardHolderLogic cardHolder)
    {
        _cardHolder = cardHolder;
        _isValid = true;
        SortBeforeValidation();
        ValidateCards();
        ContinueValidation();
    }

    private void SortBeforeValidation()
    {
        for (int i = 0; i < _cardHolder.PCardScripts.Count; i++)
        {
            for (int j = i + 1; j < _cardHolder.PCardScripts.Count; j++)
            {
                if (_cardHolder.PCardScripts[i].Card.power > _cardHolder.PCardScripts[j].Card.power)
                {
                    (_cardHolder.PCardScripts[i], _cardHolder.PCardScripts[j]) = (_cardHolder.PCardScripts[j], _cardHolder.PCardScripts[i]);
                }
            }
        }
    }

    protected virtual void ValidateCards() { }

    // Limits for player, can't use cards to get rid of them, if they can't attack enemy
    private void ContinueValidation()
    {
        if (!_isValid)
        {
            _cardHolder.OnValidationFailed();
            return;
        }

        if(PlayerCardManager.Instance.SelectedCardsPower >= EnemyCardManager.Instance.CardsInGamePower[_cardHolder.EnemyComboIndex])
        {
            _cardHolder.OnValidated();
            return;
        }

        if (_enemyAttackManager.RoundsForCards[_cardHolder.EnemyComboIndex] > 1)
        {
            _cardHolder.OnValidationFailed();
            return;
        }

        _cardHolder.OnValidated();
 
    }
}
