using UnityEngine;

public class ValidateClean : MonoBehaviour, IValidate
{
    public void ValidateCards(CardHolderManager cardHolder, Validator validator)
    {
        bool isValid = true;

        if (PlayerCardManager.Instance.SelectedCards.Count > 1)
        {
            isValid = cardHolder.PCardScript[0].Card.suit == cardHolder.PCardScript[cardHolder.PCardScript.Count - 1].Card.suit;
            Debug.Log("valid or not: " + isValid);

            for (int i = 0; i < cardHolder.PCardScript.Count - 1; i++)
            {
                if (!isValid) break;

                isValid = cardHolder.PCardScript[i].Card.suit == cardHolder.PCardScript[i + 1].Card.suit;

                if (!isValid) break;

                isValid = cardHolder.PCardScript[i].Card.power + 1 == cardHolder.PCardScript[i + 1].Card.power;
                Debug.Log("valid or not: " + isValid);
            }
        }

        Debug.Log("valid or not: " + isValid);
        validator.ContinueValidation(isValid, cardHolder);
    }

}
