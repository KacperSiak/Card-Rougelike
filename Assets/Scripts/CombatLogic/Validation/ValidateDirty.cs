using UnityEngine;

public class ValidateDirty : Validator
{
    protected override void ValidateCards()
    {
        if (PlayerCardManager.Instance.SelectedCards.Count == 1)
        {
            _isValid = false;
            Debug.Log("valid or not: " + _isValid);
            return;
        }

        _isValid = _cardHolder.PCardScripts[0].Card.suit == _cardHolder.PCardScripts[_cardHolder.PCardScripts.Count - 1].Card.suit;
        Debug.Log("valid or not clean first check: " + _isValid);

        for (int i = 0; i < _cardHolder.PCardScripts.Count - 1; i++)
        {
            if (!_isValid) break;

            _isValid = _cardHolder.PCardScripts[i].Card.suit == _cardHolder.PCardScripts[i + 1].Card.suit;

            if (!_isValid) break;

            _isValid = _cardHolder.PCardScripts[i].Card.power + 1 == _cardHolder.PCardScripts[i + 1].Card.power;
            Debug.Log("valid or not: " + _isValid);
        }

        Debug.Log("valid or not after clean check: " + _isValid);

        if (!_isValid)
        {
            _isValid = _cardHolder.PCardScripts[0].Card.suit != _cardHolder.PCardScripts[_cardHolder.PCardScripts.Count - 1].Card.suit;
            Debug.Log("valid or not dirty first check: " + _isValid);

            for (int i = 0; i < _cardHolder.PCardScripts.Count - 1; i++)
            {
                if (!_isValid) break;

                _isValid = _cardHolder.PCardScripts[i].Card.suit != _cardHolder.PCardScripts[i + 1].Card.suit;
                Debug.Log($"one {_cardHolder.PCardScripts[i].Card.suit}, two {_cardHolder.PCardScripts[i + 1].Card.suit}");

                if (!_isValid) break;

                _isValid = _cardHolder.PCardScripts[i].Card.power == _cardHolder.PCardScripts[i + 1].Card.power;
                Debug.Log($"one {_cardHolder.PCardScripts[i].Card.power}, two {_cardHolder.PCardScripts[i + 1].Card.power}");
                Debug.Log("valid or not: " + _isValid);
            }
        }

        Debug.Log("valid or not: " + _isValid);
    }
}
