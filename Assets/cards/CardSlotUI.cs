using UnityEngine;
using UnityEngine.UI;
public class CardSlotUI : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private Button _useButton;
    [SerializeField] private Button _discardButton;

    private ConsumablesData cardData;

    public void Setup(ConsumablesData data)
    {
        cardData = data;

        _icon.sprite = cardData.icon;

        _useButton.onClick.RemoveAllListeners();
        _useButton.onClick.AddListener(UseCard);

        _discardButton.onClick.RemoveAllListeners(); 
        _discardButton.onClick.AddListener(DiscardCard);
    }

    public void UseCard()
    {
        var ctx = new ConsumableContext
        {
            player = PlayerOverlay.instance
        };
        foreach (var effect in cardData.cardEffects)
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