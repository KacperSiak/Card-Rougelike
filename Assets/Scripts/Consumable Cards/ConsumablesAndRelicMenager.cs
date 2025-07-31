using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

public class ConsumablesAndRelicMenager : MonoBehaviour
{
    [Header ("Relics")]
    [SerializeField] private RelicUIDisplay _relicUI;
    public List<RelicData> currentRelics;
    public List<RelicData> allRelics;
    public int maxRelics = 5;
    private RelicContext _ctx;
    private RelicData _drawnRelic;

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
        if (RelicChecker(relic) && currentRelics.Count <maxRelics)
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

    public RelicData RollRandomRelic()
    {
        do
        {
            int index = Random.Range(0, allRelics.Count);
            _drawnRelic = allRelics[index];
        }while (!RelicChecker(_drawnRelic));

        return _drawnRelic;
    }

    public RelicData RollLegendaryRelic()
    {
        List<RelicData> legendaryRelics = new List<RelicData>();
        for (int i = 0; i < allRelics.Count; i++)
        {
            if (allRelics[i].rarity == Rarity.Legendary)
            {
                legendaryRelics.Add(allRelics[i]);
            }
        }

        do
        {
            int index = Random.Range(0, legendaryRelics.Count);
            _drawnRelic = legendaryRelics[index];
        } while (!RelicChecker(_drawnRelic));

        return _drawnRelic;
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
