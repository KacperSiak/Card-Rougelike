using UnityEngine;

public class ValidateDirty : MonoBehaviour, IValidate
{
    public void ValidateCards(CardHolderManager cardHolder, Validator validator)
    {
        bool isValid = true;

        if (PlayerCardManager.Instance.SelectedCards.Count == 1)
        {
            isValid = false;
            Debug.Log("valid or not: " + isValid);
            validator.ContinueValidation(isValid, cardHolder);
            return;
        }

        isValid = cardHolder.PCardScripts[0].Card.suit == cardHolder.PCardScripts[cardHolder.PCardScripts.Count - 1].Card.suit;
        Debug.Log("valid or not clean first check: " + isValid);

        for (int i = 0; i < cardHolder.PCardScripts.Count - 1; i++)
        {
            if (!isValid) break;

            isValid = cardHolder.PCardScripts[i].Card.suit == cardHolder.PCardScripts[i + 1].Card.suit;

            if (!isValid) break;

            isValid = cardHolder.PCardScripts[i].Card.power + 1 == cardHolder.PCardScripts[i + 1].Card.power;
            Debug.Log("valid or not: " + isValid);
        }

        Debug.Log("valid or not after clean check: " + isValid);

        if (!isValid)
        {
            isValid = cardHolder.PCardScripts[0].Card.suit != cardHolder.PCardScripts[cardHolder.PCardScripts.Count - 1].Card.suit;
            Debug.Log("valid or not dirty first check: " + isValid);

            for (int i = 0; i < cardHolder.PCardScripts.Count - 1; i++)
            {
                if (!isValid) break;

                isValid = cardHolder.PCardScripts[i].Card.suit != cardHolder.PCardScripts[i + 1].Card.suit;
                Debug.Log($"one {cardHolder.PCardScripts[i].Card.suit}, two {cardHolder.PCardScripts[i + 1].Card.suit}");

                if (!isValid) break;

                isValid = cardHolder.PCardScripts[i].Card.power == cardHolder.PCardScripts[i + 1].Card.power;
                Debug.Log($"one {cardHolder.PCardScripts[i].Card.power}, two {cardHolder.PCardScripts[i + 1].Card.power}");
                Debug.Log("valid or not: " + isValid);
            }
        }

        Debug.Log("valid or not: " + isValid);
        validator.ContinueValidation(isValid, cardHolder);
    }
}
