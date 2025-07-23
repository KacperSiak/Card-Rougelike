using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class Shop : MonoBehaviour
{
    [SerializeField] private List<ShopItem> _allItems;
    [SerializeField] private List<GameObject> _slots;

    [SerializeField] private Button _rerollButton;
    
    private int _numberOfItems = 3;
    private int _price;

    private void Start()
    {
        Debug.Log("Shop Start");
        _rerollButton.onClick.AddListener(TicketRerollShop);
        RerollShop();
    }

    public void RerollShop()
    {
        List<ShopItem> currentItems;
        do
        {
            currentItems = GetRandomItems(_allItems, _numberOfItems);
        }
        while (currentItems.TrueForAll(i => i is RelicData) || currentItems.TrueForAll(i => i is ConsumablesData) || currentItems.TrueForAll(i => i is NormalCard));

        for (int i = 0; i < currentItems.Count; i++)
        {
            var item = currentItems[i];
            var slotObject = _slots[i];
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

            shopSlot.Setup(item, _price);
        } 
    }

    private void TicketRerollShop()
    {
        if (PlayerOverlay.instance.rollTickets >= 1)
        {
            RerollShop();
            PlayerOverlay.instance.rollTickets--;
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
}
