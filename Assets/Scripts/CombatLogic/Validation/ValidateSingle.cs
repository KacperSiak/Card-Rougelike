using UnityEngine;

public class ValidateSingle : Validator
{
    protected override void ValidateCards()
    {
        if (PlayerCardManager.Instance.SelectedCards.Count > 1)
        {
            _isValid = _cardHolder.PCardScripts[0].Card.suit != _cardHolder.PCardScripts[_cardHolder.PCardScripts.Count - 1].Card.suit;
            if(_cardHolder.PCardScripts[_cardHolder.PCardScripts.Count - 1].Card.suit == Suits.Joker) _isValid = true; 

            Debug.Log("valid or not: " + _isValid);

            for (int i = 0; i < _cardHolder.PCardScripts.Count - 1; i++)
            {
                if (_cardHolder.PCardScripts[i].Card.suit == Suits.Joker)
                { 
                    _isValid = true;
                    continue;
                }
                
                if (!_isValid) break;

                _isValid = _cardHolder.PCardScripts[i].Card.suit != _cardHolder.PCardScripts[i + 1].Card.suit;
                if(_cardHolder.PCardScripts[i + 1].Card.suit == Suits.Joker) _isValid = true;

                if (!_isValid) break;

                _isValid = _cardHolder.PCardScripts[i].Card.power == _cardHolder.PCardScripts[i + 1].Card.power;
                if (_cardHolder.PCardScripts[i + 1].Card.suit == Suits.Joker) _isValid = true;
            }
        }

        Debug.Log("valid or not: " + _isValid);
    }
}
