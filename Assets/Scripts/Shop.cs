using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class Shop : MonoBehaviour
{
    public List<ShopItem> allItems;
    public List<GameObject> slots;

    [SerializeField] private Button rerollButton;
    
    private int numberOfItems = 3;
    private int price;

    private void Start()
    {
        Debug.Log("Shop Start");
        rerollButton.onClick.AddListener(RerollShop);
        RerollShop();
    }

    public void RerollShop()
    {
        List<ShopItem> currentItems;
        do
        {
            currentItems = GetRandomItems(allItems, numberOfItems);
        }
        while (currentItems.TrueForAll(i => i is RelicData) || currentItems.TrueForAll(i => i is ConsumablesData) || currentItems.TrueForAll(i => i is NormalCard));

        for (int i = 0; i < currentItems.Count; i++)
        {
            var item = currentItems[i];
            var slotObject = slots[i];
            var shopSlot = slotObject.GetComponent<ShopSlotUI>();

            if (item is RelicData relic)
            {
                switch (relic.rarity)
                {
                    case Rarity.Common: price = Random.Range(5, 10); break;
                    case Rarity.Rare: price = Random.Range(15, 20); break;
                    case Rarity.Epic: price = Random.Range(25, 30); break;
                    case Rarity.Legendary: price = Random.Range(40, 50); break;
                }
            }
            else if (item is NormalCard normal) price = Random.Range(5, 10);
            else price = Random.Range(10, 20);

            shopSlot.Setup(item, price);
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
