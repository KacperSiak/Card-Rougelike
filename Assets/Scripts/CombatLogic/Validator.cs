using UnityEngine;

public class Validator : MonoBehaviour
{
    //Refactor to Interface, remove switch, separate logic for each type of combo 

    [SerializeField] private EnemyAttackManager _enemyAttackManager;

    public void ValidateCards(CardHolderManager cardHolder)
    {
        switch (cardHolder.ComboType)
        {
            case ComboTypes.Clean:
                ValidateClean(cardHolder);
                break;
            case ComboTypes.Dirty:
                ValidateDirty(cardHolder);
                break;
            case ComboTypes.Single:
                ValidateSingle(cardHolder);
                break;
        }
    }

    private void ValidateClean(CardHolderManager cardHolder)
    {
        bool isValid = true;

        if (PlayerCardManager.Instance.SelectedCards.Count > 1)
        {
            isValid = cardHolder.PCardScript[0].Card.suit == cardHolder.PCardScript[cardHolder.PCardScript.Count - 1].Card.suit;
            Debug.Log("valid or not: " + isValid);

            for (int i = 0; i < cardHolder.PCardScript.Count - 2; i++)
            {
                if (!isValid) break;

                isValid = cardHolder.PCardScript[i].Card.suit == cardHolder.PCardScript[i + 1].Card.suit;
                isValid = cardHolder.PCardScript[i].Card.power + 1 == cardHolder.PCardScript[i + 1].Card.power;
                Debug.Log("valid or not: " + isValid);
            }
        }

        Debug.Log("valid or not: " + isValid);
        ContinueValidation(isValid, cardHolder);
    }

    private void ValidateDirty(CardHolderManager cardHolder)
    {
        bool isValid = true;

        if (PlayerCardManager.Instance.SelectedCards.Count == 1)
        {
            isValid = false;
            Debug.Log("valid or not: " + isValid);
            ContinueValidation(isValid, cardHolder);
            return;
        }

        isValid = cardHolder.PCardScript[0].Card.suit == cardHolder.PCardScript[cardHolder.PCardScript.Count - 1].Card.suit;
        Debug.Log("valid or not: " + isValid);

        for (int i = 0; i < cardHolder.PCardScript.Count - 2; i++)
        {
            if (!isValid) break;

            isValid = cardHolder.PCardScript[i].Card.suit == cardHolder.PCardScript[i + 1].Card.suit;
            isValid = cardHolder.PCardScript[i].Card.power + 1 == cardHolder.PCardScript[i + 1].Card.power;
            Debug.Log("valid or not: " + isValid);
        }

        Debug.Log("valid or not: " + isValid);

        if (!isValid)
        {
            isValid = cardHolder.PCardScript[0].Card.suit != cardHolder.PCardScript[cardHolder.PCardScript.Count - 1].Card.suit;
            Debug.Log("valid or not: " + isValid);

            for (int i = 0; i < cardHolder.PCardScript.Count - 2; i++)
            {
                if (!isValid) break;

                isValid = cardHolder.PCardScript[i].Card.suit != cardHolder.PCardScript[i + 1].Card.suit;
                isValid = cardHolder.PCardScript[i].Card.power == cardHolder.PCardScript[i + 1].Card.power;
                Debug.Log("valid or not: " + isValid);
            }
        }

        Debug.Log("valid or not: " + isValid);
        ContinueValidation(isValid, cardHolder);
    }

    private void ValidateSingle(CardHolderManager cardHolder)
    {
        bool isValid = true;

        if (PlayerCardManager.Instance.SelectedCards.Count > 1)
        {
            isValid = cardHolder.PCardScript[0].Card.suit != cardHolder.PCardScript[cardHolder.PCardScript.Count - 1].Card.suit;
            Debug.Log("valid or not: " + isValid);

            for (int i = 0; i < cardHolder.PCardScript.Count - 2; i++)
            {
                if (!isValid) break;

                isValid = cardHolder.PCardScript[i].Card.suit != cardHolder.PCardScript[i + 1].Card.suit;
                isValid = cardHolder.PCardScript[i].Card.power == cardHolder.PCardScript[i + 1].Card.power;
            }
        }

        Debug.Log("valid or not: " + isValid);
        ContinueValidation(isValid, cardHolder);
    }

    // Limits for player, can't use cards to get rig of them, if they can't attack enemy
    private void ContinueValidation(bool valid, CardHolderManager cardHolder)
    {
        if (!valid)
        {
            cardHolder.OnValidationFailed();
            Debug.Log("Validation failed");
            return;
        }

        if(PlayerCardManager.Instance.SelectedCardsPower >= EnemyCardManager.Instance.CardsInGamePower[cardHolder.EnemyCombo])
        {
            cardHolder.OnValidated();
            Debug.Log("Validation succeeded");
            return;
        }

        if (_enemyAttackManager.RoundsForCards[cardHolder.EnemyCombo] > 1)
        {
            cardHolder.OnValidationFailed();
            Debug.Log("Validation failed");
            return;
        }

        cardHolder.OnValidated();
        Debug.Log("Validation succeeded");
 
    }
}
