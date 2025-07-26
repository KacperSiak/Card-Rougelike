using UnityEngine;
using UnityEngine.UI;
public class CardSlotUI : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private Button _showButton;
    [SerializeField] private Button _useButton;
    [SerializeField] private Button _discardButton;

    private ConsumablesData _cardData;

    public void Setup(ConsumablesData data)
    {
        _cardData = data;

        _icon.sprite = _cardData.icon;

        _useButton.onClick.RemoveAllListeners();
        _useButton.onClick.AddListener(UseCard);

        _discardButton.onClick.RemoveAllListeners(); 
        _discardButton.onClick.AddListener(DiscardCard);

        _showButton.onClick.RemoveAllListeners();
        _showButton.onClick.AddListener(ShowButtons);
    }

    public void UseCard()
    {
        var ctx = new ConsumableContext
        {
            player = PlayerOverlay.instance
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
    
    public void ShowButtons()
    {
        bool _areButtonsVisible = _useButton.gameObject.activeSelf;
        _useButton.gameObject.SetActive(!_areButtonsVisible);
        _discardButton.gameObject.SetActive(!_areButtonsVisible);
    }
}