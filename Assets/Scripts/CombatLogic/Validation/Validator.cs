using UnityEngine;

public class Validator : MonoBehaviour
{
    //Refactor to abstract class T_T

    [SerializeField] private EnemyAttackManager _enemyAttackManager;

    public void ValidateCards(CardHolderManager cardHolder)
    {
        cardHolder.ValidatorType.ValidateCards(cardHolder, this);
    }

    // Limits for player, can't use cards to get rig of them, if they can't attack enemy
    public void ContinueValidation(bool valid, CardHolderManager cardHolder)
    {
        if (!valid)
        {
            cardHolder.OnValidationFailed();
            Debug.Log("Validation failed");
            return;
        }

        if(PlayerCardManager.Instance.SelectedCardsPower >= EnemyCardManager.Instance.CardsInGamePower[cardHolder.EnemyComboIndex])
        {
            cardHolder.OnValidated();
            Debug.Log("Validation succeeded");
            return;
        }

        if (_enemyAttackManager.RoundsForCards[cardHolder.EnemyComboIndex] > 1)
        {
            cardHolder.OnValidationFailed();
            Debug.Log("Validation failed");
            return;
        }

        cardHolder.OnValidated();
        Debug.Log("Validation succeeded");
 
    }
}
