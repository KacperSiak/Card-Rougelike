using UnityEngine;
using UnityEngine.UI;
public class CardSlotUI : MonoBehaviour
{
    [SerializeField] private Image _icon;
    public Button useButton;
    public Button discardButton;

    //prywatne zmienne _nazwa
    private ConsumablesData _cardData;
    private PlayerOverlay _player;

    public void Setup(ConsumablesData data, PlayerOverlay p)
    {
        _cardData = data;
        _player = p;

        _icon.sprite = _cardData.icon;

        useButton.onClick.RemoveAllListeners();
        useButton.onClick.AddListener(UseCard);

        discardButton.onClick.RemoveAllListeners(); 
        discardButton.onClick.AddListener(DiscardCard);
    }

    public void UseCard()
    {
        var ctx = new ConsumableContext
        {
            player = _player
        };
        foreach (var effect in _cardData.cardEffects)
        {
            effect.ApplyEffect(ctx);
        }

        Destroy(gameObject);
    }

    public void DiscardCard()
    {
        Destroy(gameObject);
    }
}