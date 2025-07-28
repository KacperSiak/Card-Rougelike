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
            if (_currentItem is RelicData relic)
            {
                RelicEventsSystem.RelicAcquired(relic);
                PlayerOverlay.instance.SpendGold(_currentPrice);
                
            }
            else if (_currentItem is ConsumablesData consumable)
            {
                if(ConsumablesAndRelicMenager.instance.SlotChecker())
                {
                    ConsumablesAndRelicMenager.instance.AddCard(consumable);
                    PlayerOverlay.instance.SpendGold(_currentPrice);
                }
            }
            else if (_currentItem is NormalCard normal)
            {
                // deckMenager.instance.Add(normal);
                PlayerOverlay.instance.SpendGold(_currentPrice);
            }
            Debug.Log("Item bought");
            gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("You cant affrod it");
        }
        
    }
}