using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class ShopMenager : PersistenSingleton<ShopMenager>
{
    [SerializeField] private List<ShopItem> items;
    public List<ShopItem> allItems;
    [Header("Slot Lists")]
    [SerializeField] private List<GameObject> _slots;
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private Transform _cardPanel;
    public List<GameObject> cardslots;

    [Header("Buttons")]
    [SerializeField] private Button _rerollButton;  
    [SerializeField] private Button _sellModeButton;
    [SerializeField] private Button _eatLutter;
    [SerializeField] private Button _Lutter;
    [SerializeField] private Button _closeLuttMenu;
    // [SerializeField] private Button _closeShop;

    [Header("Rest")]
    [SerializeField] private TextMeshProUGUI _rerollText;
    [SerializeField] private GameObject _lutterPanel;

    private int _numberOfItems = 3;
    private int _price;
    private float _priceMultiplier = 1f;
    private bool _freeBuyEffect = false;
    private int _goldForRerol;
    private List<BaseCard> _cardsForVisual;

    public bool inSellingMode { get; private set; } = false;

    public static UnityEvent OnSellingMode = new UnityEvent();
    public static UnityEvent OffSellingMode = new UnityEvent();
    protected override void OnSingletonAwake() 
    {
        _rerollButton.onClick.AddListener(RerollForGold);
        _sellModeButton.onClick.AddListener(SellingMode);
        _eatLutter.onClick.AddListener(EatLutter);
        _Lutter.onClick.AddListener(OpenMenu);
        _closeLuttMenu.onClick.AddListener(CloseMenu);
        allItems = items;
        //_closeShop.onClick.AddListener(CloseShop);
    }

    protected override void OnDestroy()
    {
        _rerollButton.onClick.RemoveListener(RerollForGold);
        _sellModeButton.onClick.RemoveListener(SellingMode);
        _eatLutter.onClick.RemoveListener(EatLutter);
        _Lutter.onClick.RemoveListener(OpenMenu);
        _closeLuttMenu.onClick.RemoveListener(CloseMenu);
        //_closeShop.onClick.RemoveListener(CloseShop);

        base.OnDestroy();
    }

    private void Start()
    {
        
        RerollShop();
        _goldForRerol = 5;
        _rerollText.text = "Reroll: " + _goldForRerol;

        _cardsForVisual = DeckManager.instance.DrawACardForShop();
        for (int i = 0; i < _cardsForVisual.Count; i++)
        {
            var card = _cardsForVisual[i];
            var cardSlot = Instantiate(_cardPrefab, _cardPanel);
            cardSlot.SetActive(false);
            var cardUI = cardSlot.GetComponent<CardShopSlotUI>();
            cardslots.Add(cardSlot);
            cardUI.Setup(card);
        }
    }

    private void OnEnable()
    {
        Discount.OnDiscountActivated.AddListener(ApplyDiscount);
        Discount.OnDiscountDeactivated.AddListener(RemoveDiscount);
    }

    private void OnDisable()
    {
        Discount.OnDiscountActivated.RemoveListener(ApplyDiscount);
        Discount.OnDiscountDeactivated.RemoveListener(RemoveDiscount);
    }

    public void RerollShop()
    {
        List<ShopItem> currentItems;
        do
        {
            currentItems = GetRandomItems(allItems, _numberOfItems);
        }
        while (currentItems.TrueForAll(i => i is RelicData) || currentItems.TrueForAll(i => i is ConsumablesData) || currentItems.TrueForAll(i => i is PotionData));

        for (int i = 0; i < currentItems.Count; i++)
        {
            var item = currentItems[i];
            var slotObject = _slots[i];
            slotObject.SetActive(true);
            var shopSlot = slotObject.GetComponent<ShopSlotUI>();

            switch (item)
            {
                case RelicData relic: 
                    switch (relic.rarity)
                    {
                        case Rarity.Common: _price = relic.buyPrice; break;
                        case Rarity.Rare: _price = relic.buyPrice; break;
                    }
                    break;

                case ConsumablesData consumable: _price = consumable.buyPrice; break;
                case CardPack cardPack: _price = cardPack.buyPrice; break;
                case VaseRelicUpgrade vase: _price = vase.price; break;
                case PotionData potion: _price = potion.buyPrice; break;
            }   
            shopSlot.Setup(item, Mathf.RoundToInt(_price * _priceMultiplier));
        }
    }

    private void RerollForGold()
    {
        foreach (var slotObj in cardslots)
            slotObj.SetActive(false);
        if (PlayerManager.Instance.gold >= _goldForRerol)
        {
            RerollShop();
            PlayerManager.Instance.GoldChange(_goldForRerol, false);
            _goldForRerol += 1;
            _rerollText.text = "Reroll: " + _goldForRerol;
            _lutterPanel.SetActive(false);
        }
        else
        {
            Debug.Log("You cannot Roll");
        }
    }

    private List<T> GetRandomItems<T>(List<T> list, int count)
    {
        List<T> copy = new List<T>(list);
        List<T> result = new List<T>();

        for (int i = 0; i < count && copy.Count > 0; i++)
        {
            int index = Random.Range(0, copy.Count);
            result.Add(copy[index]);
            copy.RemoveAt(index);
        }

        return result;
    }

    private void ApplyDiscount(float discount)
    {
        _priceMultiplier = discount;
        UpdatePrices(_priceMultiplier, false);
    }

    private void RemoveDiscount()
    {
        _priceMultiplier = 1f;
        UpdatePrices(_priceMultiplier, true);
    }

    public void UpdatePrices(float multiplayer, bool basic)
    {
        foreach (var slotObj in _slots)
        {
            if (!slotObj.activeSelf) continue;

            if (!basic)
            {
                var shopSlot = slotObj.GetComponent<ShopSlotUI>();
                shopSlot.RecalculatePrice(multiplayer, false);
            }
            else
            {
                var shopSlot = slotObj.GetComponent<ShopSlotUI>();
                shopSlot.RecalculatePrice(multiplayer, true);
            }
        }
    }

    public void ActiveFreBuy()
    {
        _freeBuyEffect = true;
        UpdatePrices(0, false);
    }

    public void ConsumeFreeBuy()
    {
        _freeBuyEffect = false;
        UpdatePrices(_priceMultiplier, false);
    }

    public bool IsFreeBuyActive()
    {
        return _freeBuyEffect;
    }

    private void SellingMode()
    {
        inSellingMode = true;
        OnSellingMode.Invoke();
        _lutterPanel.SetActive(false);
        foreach (var slotObj in _slots)
            slotObj.SetActive(false);

        foreach (var slot in cardslots)
            slot.SetActive(true);
    }

    private void CloseShop()
    {
        foreach (var slotObj in cardslots)
            slotObj.SetActive(false);
        inSellingMode = false;
        OffSellingMode.Invoke();
    }

    private void EatLutter()
    {
        foreach (var slotObj in cardslots)
            slotObj.SetActive(false);
        inSellingMode = false;
        OffSellingMode.Invoke();
        _lutterPanel.SetActive(false);
        foreach ( var slotObj in _slots)
        {
            if(!slotObj.activeSelf) slotObj.SetActive(true);
            var shopslot = slotObj.GetComponent<ShopSlotUI>();
            shopslot.RecalculatePrice(9999, false);
        }
    }

    private void OpenMenu()
    {
        inSellingMode = false;
        OffSellingMode.Invoke();
        _lutterPanel.SetActive(true);
    }

    private void CloseMenu()
    {
        inSellingMode=false;
        OffSellingMode.Invoke();
        _lutterPanel.SetActive(false);
    }
}

