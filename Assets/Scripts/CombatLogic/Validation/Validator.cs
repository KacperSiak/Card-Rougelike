using UnityEngine;

//jocker validation to be improved 
public class Validator : MonoBehaviour
{
    [SerializeField] protected EnemyAttackManager _enemyAttackManager;
    protected bool _isValid = false;
    protected CardHolderManager _cardHolder;

    public void Validate(CardHolderManager cardHolder)
    {
        _cardHolder = cardHolder;
        _isValid = true;
        ValidateCards();
        ContinueValidation();
    }

    protected virtual void ValidateCards() { }

    // Limits for player, can't use cards to get rig of them, if they can't attack enemy
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
