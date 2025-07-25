using UnityEngine;

public class ValidateClean : MonoBehaviour, IValidate
{
    public void ValidateCards(CardHolderManager cardHolder, Validator validator)
    {
        bool isValid = true;

        if (PlayerCardManager.Instance.SelectedCards.Count > 1)
        {
            isValid = cardHolder.PCardScripts[0].Card.suit == cardHolder.PCardScripts[cardHolder.PCardScripts.Count - 1].Card.suit;
            Debug.Log("valid or not: " + isValid);

            for (int i = 0; i < cardHolder.PCardScripts.Count - 1; i++)
            {
                if (!isValid) break;

                isValid = cardHolder.PCardScripts[i].Card.suit == cardHolder.PCardScripts[i + 1].Card.suit;

                if (!isValid) break;

                isValid = cardHolder.PCardScripts[i].Card.power + 1 == cardHolder.PCardScripts[i + 1].Card.power;
                Debug.Log("valid or not: " + isValid);
            }
        }

        Debug.Log("valid or not: " + isValid);
        validator.ContinueValidation(isValid, cardHolder);
    }

}
