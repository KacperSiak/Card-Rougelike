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

        if (item is RelicData relic) _icon.sprite = relic.icon;
        else if (item is ConsumablesData consumable) _icon.sprite = consumable.icon;
        else if (item is NormalCard normal) _icon.sprite = normal.icon;
        else if (item is VaseRelicUpgrade vase) _icon.sprite = vase.icon;
        else if (item is PotionData potion) _icon.sprite = potion.potionIcon;
        _priceText.text = $"{price}";
    }

    public void Buy()
    {
        if (ShopMenager.Instance.IsFreeBuyActive())
        {
            _currentPrice = 0;
            ShopMenager.Instance.ConsumeFreeBuy();
        }
        if (PlayerOverlay.Instance.gold >= _currentPrice)
        {
            switch (_currentItem)
            {
                case RelicData relic:
                    PlayerOverlay.Instance.SpendGold(_currentPrice);
                    RelicEventsSystem.RelicAcquired(relic);
                    break;

                case ConsumablesData consumable when ConsumablesAndRelicMenager.Instance.SlotChecker():
                    ConsumablesAndRelicMenager.Instance.AddCard(consumable);
                    PlayerOverlay.Instance.SpendGold(_currentPrice);
                    break;

                case CardPack cardPack:
                    PlayerOverlay.Instance.SpendGold(_currentPrice);
                    cardPack.OpePack();
                    break;

                case VaseRelicUpgrade vase:
                    PlayerOverlay.Instance.SpendGold(_currentPrice);
                    vase.GotVase();
                    break;

                case PotionData potion:
                    PlayerOverlay.Instance.SpendGold(_currentPrice);
                    BuffsAndDebuffsManager.Instance.ApplyBuff(potion);
                    break;

                default:
                    Debug.LogWarning("Nieznany typ itemu: " + _currentItem);
                    break;
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