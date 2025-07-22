using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlotUI : MonoBehaviour
{
    [SerializeField] private Button buyButton;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI priceText;

    private ScriptableObject currentItem;
    private int currentPrice;
    void Start()
    {
        buyButton.onClick.AddListener(Buy);
    }

    public void Setup(ScriptableObject item, int price)
    {
        currentItem = item;
        currentPrice = price;

        if (item is RelicData relic)
        {
            icon.sprite = relic.icon;
        }
        else if (item is ConsumablesData consumable)
        {
            icon.sprite = consumable.icon;
        }
        else if (item is NormalCard normal)
        {
            icon.sprite = normal.face;
        }
        priceText.text = $"{price}";
    }

    public void Buy()
    {
        if (PlayerOverlay.instance.gold >= currentPrice)
        {
            PlayerOverlay.instance.SpendGold(currentPrice);
        }
    }
}
