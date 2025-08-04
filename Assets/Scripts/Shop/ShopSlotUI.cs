using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ShopSlotUI : MonoBehaviour
{
    [SerializeField] private Button _buyButton;
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _priceText;

    private ScriptableObject _currentItem;
    private int _currentPrice;
    private int _basePrice;

    void Start()
    {
        _buyButton.onClick.AddListener(Buy);
    }

    public void Setup(ScriptableObject item, int price)
    {
        _currentItem = item;
        _currentPrice = price;
        _basePrice = price;

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
            _icon.sprite = normal.icon;
        }
        else if (item is VaseRelicUpgrade vase)
        {
            _icon.sprite = vase.icon;
        }
        _priceText.text = $"{price}";
    }

    public void Buy()
    {
        if (ShopMenager.instance.IsFreeBuyActive())
        {
            _currentPrice = 0;
            ShopMenager.instance.ConsumeFreeBuy();
        }
        if (PlayerOverlay.instance.gold >= _currentPrice)
        {
            if (_currentItem is RelicData relic)
            {
                PlayerOverlay.instance.SpendGold(_currentPrice);
                RelicEventsSystem.RelicAcquired(relic);

            }
            else if (_currentItem is ConsumablesData consumable)
            {
                if(ConsumablesAndRelicMenager.instance.SlotChecker())
                {
                    ConsumablesAndRelicMenager.instance.AddCard(consumable);
                    PlayerOverlay.instance.SpendGold(_currentPrice);
                }
            }
            else if (_currentItem is CardPack cardPack)
            {
                PlayerOverlay.instance.SpendGold(_currentPrice);
                cardPack.OpePack();
            }
            else if (_currentItem is VaseRelicUpgrade vase)
            {
                PlayerOverlay.instance.SpendGold(_currentPrice);
                vase.GotVase();
            }
            Debug.Log("Item bought");
            gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("You cant affrod it");
        }
        
    }

    public void RecalculatePrice(float multiplayer, bool basic)
    {
        if (!basic)
        {
            if (multiplayer == 9999) _currentPrice = 99999;
            else _currentPrice = Mathf.RoundToInt(_basePrice * multiplayer);
        }
        else _currentPrice = _basePrice;

        _priceText.text = $"{_currentPrice}";
    }
}