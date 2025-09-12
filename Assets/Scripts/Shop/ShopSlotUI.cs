using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShopSlotUI : MonoBehaviour
{
    [SerializeField] private Button _buyButton;
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _priceText;
    [SerializeField] private TextMeshProUGUI _descritpion;
    [SerializeField] private Button _scrollButton;

    private ScriptableObject _currentItem;
    private int _currentPrice;
    private int _basePrice;
    private Animator _anim;
    private bool _opened = false;

    void Start()
    {
        _buyButton.onClick.AddListener(Buy);
        _buyButton.gameObject.SetActive(false);
        _descritpion.gameObject.SetActive(false);
        _scrollButton.onClick.AddListener(ChangeScroll);
        _anim = GetComponentInChildren<Animator>();
    }

    private void ChangeScroll()
    {
        if(!_opened)
        {
            StartCoroutine(OpenThenShow());
        }
    }

    private System.Collections.IEnumerator OpenThenShow()
    {
        _anim.SetTrigger("Open");
        yield return new WaitForSeconds(0.33f);
        _buyButton.gameObject.SetActive(true);
        _descritpion.gameObject.SetActive(true);
        _opened = true;
    }

    public void Setup(ScriptableObject item, int price)
    {
        _currentItem = item;
        _currentPrice = price;
        _basePrice = price;


        switch (item)
        {
            case RelicData relic:
                _icon.sprite = relic.icon;
                _descritpion.text = relic.description;
                break;

            case ConsumablesData consumable:
                _icon.sprite = consumable.icon; 
                _descritpion.text = consumable.cardDescription;
                break;

            case NormalCard normal:
                _icon.sprite = normal.icon; break;

            case VaseRelicUpgrade vase:
                _icon.sprite = vase.icon;
                _descritpion.text = "Vase you can use for Relic upgrade";
                break;

            case PotionData potion:
                _icon.sprite = potion.potionIcon; 
                _descritpion.text = potion.potionDescription;
                break;
        }
 
        _priceText.text = $"{price}";
    }

    public void Buy()
    {
        if (ShopMenager.Instance.IsFreeBuyActive())
        {
            _currentPrice = 0;
            ShopMenager.Instance.ConsumeFreeBuy();
        }
        if (PlayerManager.Instance.gold >= _currentPrice)
        {
            switch (_currentItem)
            {
                case RelicData relic when ConsumablesAndRelicMenager.Instance.RelicSlotCounter():
                    PlayerManager.Instance.GoldChange(_currentPrice, false);
                    RelicEventsSystem.RelicAcquired(relic);
                    gameObject.SetActive(false);
                    break;

                case ConsumablesData consumable when ConsumablesAndRelicMenager.Instance.SlotChecker():
                    ConsumablesAndRelicMenager.Instance.AddCard(consumable);
                    PlayerManager.Instance.GoldChange(_currentPrice, false);
                    gameObject.SetActive(false);
                    break;

                case CardPack cardPack:
                    PlayerManager.Instance.GoldChange(_currentPrice, false);
                    cardPack.OpenPack();
                    gameObject.SetActive(false);
                    break;

                case VaseRelicUpgrade vase:
                    PlayerManager.Instance.GoldChange(_currentPrice, false);
                    vase.GotVase();
                    gameObject.SetActive(false);
                    break;

                case PotionData potion:
                    PlayerManager.Instance.GoldChange(_currentPrice, false);
                    BuffsAndDebuffsManager.Instance.ApplyBuff(potion);
                    gameObject.SetActive(false);
                    break;

                default:
                    Debug.LogWarning("Nieznany typ itemu: " + _currentItem);
                    break;
            }
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