using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IconButtonUI : MonoBehaviour
{
    [SerializeField] private RelicData _relic;
    [SerializeField] private Button _removeButton;
    [SerializeField] private TextMeshProUGUI _buttonText;

    private bool _showingRemove = false;

    private void Awake()
    {
        ShopMenager.OnShopOpen.AddListener(ButtonDiscardToSell);
        ShopMenager.OnShopClose.AddListener(ButtonSellToDiscard);
    }
    public void Init(RelicData relic)
    {
        _relic = relic;
        _removeButton.gameObject.SetActive(false);
        _removeButton.onClick.AddListener(RemoveRelic);

        if(ShopMenager.instance.inShop)
        {
            _buttonText.text = $"Sell {relic.sellPrice}";
        }
    }

    public void ToggleRemoveButton()
    {
        _showingRemove = !_showingRemove;
        _removeButton.gameObject.SetActive(_showingRemove);
    }

    public void RemoveRelic()
    {
        if (ShopMenager.instance.inShop)
        {
            PlayerOverlay.instance.AddGold(_relic.sellPrice);
        }
        ToolTipManager.Instance.HideToolTip();
        RelicEventsSystem.RelicRemoved(_relic);
        Destroy(gameObject);
    }

    private void ButtonDiscardToSell()
    {
        _buttonText.text = "Sell";
    }

    public void ButtonSellToDiscard()
    {
        _buttonText.text = "X";
    }
}
