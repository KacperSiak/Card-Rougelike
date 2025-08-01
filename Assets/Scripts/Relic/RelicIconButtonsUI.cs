using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RelicIconButtonsUI : MonoBehaviour
{
    [SerializeField] private RelicData _relic;
    [SerializeField] private Button _removeButton;
    [SerializeField] private Button _vaseButton;
    [SerializeField] private TextMeshProUGUI _buttonText;

    private bool _showingRemove = false;
    public static bool vaseEnabled = false;

    public static UnityEvent<RelicData> OnRelicUpgrade = new UnityEvent<RelicData>();

    private void Awake()
    {
        ShopMenager.OnSellingMode.AddListener(ButtonDiscardToSell);
        ShopMenager.OffSellingMode.AddListener(ButtonSellToDiscard);
        VaseRelicUpgrade.OnVaseAcquired.AddListener(VaseButtonShow);
        
    }
    public void Init(RelicData relic)
    {
        _relic = relic;
        _removeButton.gameObject.SetActive(false);
        _vaseButton.gameObject.SetActive(vaseEnabled);
        _removeButton.onClick.AddListener(RemoveRelic);
        _vaseButton.onClick.AddListener(UpgradeRelic);

        if(ShopMenager.instance.inSellingMode)
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
        if (ShopMenager.instance.inSellingMode)
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

    private void VaseButtonShow()
    {
        if (_relic.Upgradeable)
            _vaseButton.gameObject.SetActive(true);
    }

    private void UpgradeRelic()
    {
        OnRelicUpgrade.Invoke(_relic);
        _vaseButton.gameObject.SetActive(false);
        vaseEnabled = false;
        ToolTipManager.Instance.HideToolTip();
        Destroy(gameObject);
    }
}
