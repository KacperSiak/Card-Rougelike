using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlotUI : MonoBehaviour
{
    [SerializeField] private Button _buyButton;
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _priceText;

    private ScriptableObject _currentItem;
    private int _currentPrice;
    void Start()
    {
        _buyButton.onClick.AddListener(Buy);
    }

    public void Setup(ScriptableObject item, int price)
    {
        _currentItem = item;
        _currentPrice = price;

        if (item is RelicData relic)
        {
            _icon.sprite = relic.icon;
        }
        else if (item is ConsumablesData consumable)
        {
            _icon.sprite = consumable.icon;
        }
        else if (item is NormalCard normal)
        {
            _icon.sprite = normal.face;
        }
        _priceText.text = $"{price}";
    }

    public void Buy()
    {
        if (PlayerOverlay.instance.gold >= _currentPrice)
        {
            PlayerOverlay.instance.SpendGold(_currentPrice);
        }
    }
}
