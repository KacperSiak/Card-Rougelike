using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class ShopMenager : MonoBehaviour
{
    public  List<ShopItem> allItems;
    [SerializeField] private List<GameObject> _slots;

    [SerializeField] private Button _rerollButton;
    [SerializeField] private TextMeshProUGUI _rerollText;
    [SerializeField] private Button _closeShop;
    
    private int _numberOfItems = 3;
    private int _price;
    private float _priceMultiplier = 1f;
    private bool _freeBuyEffect = false;
    private int _goldForRerol;

    public static ShopMenager instance;
    public bool inShop { get; private set; } = false;

    public static UnityEvent OnShopOpen = new UnityEvent();
    public static UnityEvent OnShopClose = new UnityEvent();
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("Found Another ShopMenager");
        }
    }

    public void OpenShop()
    {
        inShop = true;
        OnShopOpen.Invoke();
    }

    private void CloseShop()
    {
        inShop = false;
        OnShopClose.Invoke();
    }


    private void Start()
    {
        _rerollButton.onClick.AddListener(RerollForGold);
        _closeShop.onClick.AddListener(CloseShop);
        RerollShop();
        _goldForRerol = 5;
        _rerollText.text = "" + _goldForRerol;
    }

    //private void Update()
    //{
    //    if (Input.GetKeyUp(KeyCode.L))
    //    {
    //        if (inShop)
    //        {
    //            CloseShop();
    //            Debug.Log("Shop Closed");
    //        }
    //        else
    //        {
    //            OpenShop();
    //            Debug.Log("Shop opened");
    //        }
    //    }
    //}

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
        while (currentItems.TrueForAll(i => i is RelicData) || currentItems.TrueForAll(i => i is ConsumablesData) || currentItems.TrueForAll(i => i is NormalCard)); 

        for (int i = 0; i < currentItems.Count; i++)
        {
            var item = currentItems[i];
            var slotObject = _slots[i];
            slotObject.SetActive(true);
            var shopSlot = slotObject.GetComponent<ShopSlotUI>();
            if (item is RelicData relic)
            {
                switch (relic.rarity)
                {
                    case Rarity.Common: _price = Random.Range(5, 10); break;
                    case Rarity.Rare: _price = Random.Range(15, 20); break;
                    case Rarity.Epic: _price = Random.Range(25, 30); break;
                    case Rarity.Legendary: _price = Random.Range(40, 50); break;
                }
            }
            else if (item is NormalCard normal) _price = Random.Range(5, 10);
            else _price = Random.Range(10, 20);
            shopSlot.Setup(item, Mathf.RoundToInt(_price * _priceMultiplier));
        } 
    }

    private void RerollForGold()
    {
        if (PlayerOverlay.instance.gold >= _goldForRerol)
        {
            RerollShop();
            PlayerOverlay.instance.SpendGold(_goldForRerol);
            _goldForRerol += 1;
            _rerollText.text = "" + _goldForRerol;
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
}
