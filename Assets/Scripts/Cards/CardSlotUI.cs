using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardSlotUI : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private Button _showButton;
    [SerializeField] private Button _useButton;
    [SerializeField] private Button _discardButton;
    [SerializeField] private TextMeshProUGUI _discardButtonText;

    private ConsumablesData _cardData;

    private void Awake()
    {
        ShopMenager.OnSellingMode.AddListener(ChangeDiscardToSell);
        ShopMenager.OffSellingMode.AddListener(ChangeSellToDiscard);
    }
   
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

        if (ShopMenager.Instance.inSellingMode)
        {
            _discardButtonText.text = $"Sell {_cardData.sellPrice}";
        }    
    }

    public void UseCard()
    {
        var ctx = new ConsumableContext
        {
            player = PlayerManager.Instance
        };
        foreach (var effect in _cardData.cardEffects)
        {
            effect.ApplyEffect(ctx);
        }
        ConsumablesAndRelicMenager.Instance.currentConsums.Remove(_cardData);
        ConsumablesAndRelicMenager.Instance.slotCounter--;
        Destroy(gameObject);
    }

    public void DiscardCard()
    {
        if (ShopMenager.Instance.inSellingMode)
        {
            PlayerManager.Instance.GoldChange(_cardData.sellPrice, true);
        }
        ConsumablesAndRelicMenager.Instance.currentConsums.Remove(_cardData);
        ConsumablesAndRelicMenager.Instance.slotCounter--;
        Destroy(gameObject);
    }

    public void ShowButtons()
    {
        bool _areButtonsVisible = _useButton.gameObject.activeSelf;
        _useButton.gameObject.SetActive(!_areButtonsVisible);
        _discardButton.gameObject.SetActive(!_areButtonsVisible);
    }

    private void ChangeDiscardToSell()
    {
        _discardButtonText.text = $"Sell {_cardData.sellPrice}";
    }

    private void ChangeSellToDiscard()
    {
        _discardButtonText.text = "Discard";
    }
}