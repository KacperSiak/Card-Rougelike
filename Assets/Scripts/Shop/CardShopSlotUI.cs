using UnityEngine;
using UnityEngine.UI;

public class CardShopSlotUI : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private Button _sellButton;
    [SerializeField] private Button _showButton;

    private BaseCard _cardToSell;
    private int price = 10;
    private void Start()
    {
        _sellButton.onClick.AddListener(SellCard);
        _showButton.onClick.AddListener(ShowSellButton);
    }

    public void Setup(BaseCard normal)
    {
        _icon.sprite = normal.icon;
        _cardToSell = normal;
    }

    private void SellCard()
    {
        if (_cardToSell != null)
        {
            DeckMenager.instance.RemmoveCard(_cardToSell);
        }

        PlayerOverlay.instance.AddGold(price);
        ShopMenager.instance.cardslots.Remove(gameObject);
        Destroy(gameObject);
    }

    private void ShowSellButton()
    {
        _sellButton.gameObject.SetActive(!_sellButton.gameObject.activeSelf);
    }
}
