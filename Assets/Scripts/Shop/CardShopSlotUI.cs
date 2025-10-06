using UnityEngine;
using UnityEngine.UI;

public class CardShopSlotUI : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private Button _sellButton;
    [SerializeField] private Button _showButton;
    [SerializeField] private Texture2D _atlasTexture;

    private int _rows = 4;
    private int _cols = 13;

    private CardsSO _cardToSell;
    private int price = 10;
    private void Start()
    {
        _sellButton.onClick.AddListener(SellCard);
        _showButton.onClick.AddListener(ShowSellButton);
    }

    public void Setup(CardsSO normal)
    {
        Sprite sp = AtlasHelper.Cutoff(
            _atlasTexture,
            _rows,
            _cols,
            normal.row,
            normal.column,
            rowZeroTop: true
        );

        _icon.sprite = sp;
        _cardToSell = normal;
    }

    private void SellCard()
    {
        if (_cardToSell != null)
        {
            RoundManager.Instance.RemoveCard(_cardToSell);
        }

        PlayerManager.Instance.GoldChange(price, true);
        ShopMenager.Instance.cardslots.Remove(gameObject);
        Destroy(gameObject);
    }

    private void ShowSellButton()
    {
        _sellButton.gameObject.SetActive(!_sellButton.gameObject.activeSelf);
    }
}
