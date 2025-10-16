using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

[System.Serializable]   
public struct MaterialsByType
{
    public ConsCardType type;
    public Material material;
}
public class ConsCardProperties : MonoBehaviour
{
    [SerializeField] private MeshRenderer _front;
    [SerializeField] private MeshRenderer _back;
    [SerializeField] private MaterialsByType[] _materials;

    [SerializeField] private Texture2D _moiraeBack;
    [SerializeField] private Texture2D _oneUseBack;

    [SerializeField] private Button _useButton;
    [SerializeField] private Button _discardButton;
    [SerializeField] private TextMeshProUGUI _discardButtonText;

    private ConsumablesData _currCons;
    private ConsumableContext _consContext;
    private bool _visibleButtons = false;

    public static UnityEvent OnConsumableUse = new UnityEvent();

    private void OnEnable()
    {
        _useButton.onClick.AddListener(Use);
        _discardButton.onClick.AddListener(Discard);
        _consContext = new ConsumableContext();
        ShopMenager.OnSellingMode.AddListener(ButtonDiscardToSell);
        ShopMenager.OffSellingMode.AddListener(ButtonSellToDiscard);
        _visibleButtons = false;
        _useButton.gameObject.SetActive(false);
        _discardButton.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        _useButton.onClick.RemoveAllListeners();
        _discardButton.onClick.RemoveAllListeners();
    }

    public void Setup(ConsumablesData cons)
    {
        _currCons = cons;
        var mat = FindMaterial(_currCons.type);
        if (mat != null) _front.sharedMaterial = mat;

        MaterialSetupManager.Instance.SetupVisual(_front, _currCons.col, _currCons.row);

        if (_currCons.type == ConsCardType.Moirae) _back.material.mainTexture = _moiraeBack;
        else _back.material.mainTexture = _oneUseBack;
        gameObject.SetActive(true);
    }

    private Material FindMaterial(ConsCardType type)
    {
        for (int i = 0; i < _materials.Length; i++)
        {
            if (_materials[i].type == type) 
                return _materials[i].material;
        }
        return null;
    }

    private void Use()
    {
        foreach (var effect in _currCons.cardEffects)
        {
            effect.ApplyEffect(_consContext);
        }
        gameObject.SetActive(false);
        _currCons = null;
        gameObject.SetActive(false);

        OnConsumableUse?.Invoke();
    }

    private void Discard()
    {
        gameObject.SetActive(false);
        _currCons = null;

        if (ShopMenager.Instance.inSellingMode)
        {
            PlayerManager.Instance.GoldChange(_currCons.sellPrice, true);
        }
    }

    private void ButtonDiscardToSell()
    {
        _discardButtonText.text = $"Sell {_currCons.sellPrice}";
    }

    public void ButtonSellToDiscard()
    {
        _discardButtonText.text = "Discard";
    }

    private void OnMouseDown()
    {
        if(_visibleButtons)
        {
            _useButton.gameObject.SetActive(false);
            _discardButton.gameObject.SetActive(false);
            _visibleButtons = false;
        }
        else
        {
            _useButton.gameObject.SetActive( true);
            _discardButton.gameObject.SetActive(true);
            _visibleButtons = true;
        }
    }
}
