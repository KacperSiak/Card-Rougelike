using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ConsumablesAndRelicMenager : MonoBehaviour
{
    [Header ("Relics")]
    [SerializeField] private RelicUIDisplay _relicUI;
    public List<RelicData> currentRelics;
    private RelicContext _ctx;

    [Header("Consumables")]
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private Transform _slotParent;
    public int slotCounter = 0;

    public static ConsumablesAndRelicMenager instance;

    private void Awake()
    {
        RelicEventsSystem.OnRelicAcquired.AddListener(HandleRelicAcquired);
        RelicEventsSystem.OnRelicRemoved.AddListener(HandleRelicRemoved);

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("Found Another ConsumableMenager");
        }
    }

    private void Start()
    {
        _ctx = new RelicContext { player = PlayerOverlay.instance };
    }

    private void OnDestroy()
    {
        RelicEventsSystem.OnRelicAcquired.RemoveListener(HandleRelicAcquired);
        RelicEventsSystem.OnRelicRemoved.RemoveListener(HandleRelicRemoved);
    }

// Relics //    
    private void HandleRelicAcquired(RelicData relic)
    {
        if (RelicChecker(relic) && currentRelics.Count <5)
        {
            currentRelics.Add(relic);
            foreach (var effect in relic.relicEffets) effect.Register(_ctx);
            _relicUI.AddIcon(relic);
        }

        ShopMenager.instance.allItems.Remove(relic);
    }

    private void HandleRelicRemoved(RelicData relic)
    {
        currentRelics.Remove(relic);
        foreach (var effect in relic.relicEffets) effect.Unregister(_ctx);

        ShopMenager.instance.allItems.Add(relic);
    }

    public bool RelicChecker(RelicData relic)
    {
        if (!currentRelics.Any(r => r.relicID == relic.relicID))
        {
            return true;
        }

        return false;
    }


// Consumables cards // 
    public void AddCard(ConsumablesData card)
    {
        if (SlotChecker())
        {   
            var slot = Instantiate(_slotPrefab, _slotParent);
            slot.GetComponent<CardSlotUI>().Setup(card);
            slotCounter++;
        }
    }

    public bool SlotChecker()
    {
        if (slotCounter < 3)
            return true;

        return false;
    }
}
