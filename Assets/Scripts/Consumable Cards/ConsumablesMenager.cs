using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ConsumablesMenager : MonoBehaviour
{
    public List<RelicData> currentRelics;
    [SerializeField] private RelicUIDisplay _relicUI;

    private RelicContext _ctx;

    public static ConsumablesMenager instance;

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

    private void HandleRelicAcquired(RelicData relic)
    {
        if (!currentRelics.Any(r => r.relicID == relic.relicID))
        {
            currentRelics.Add(relic);
            foreach (var effect in relic.relicEffets) effect.Register(_ctx);
            _relicUI.AddIcon(relic);
        }
    }

    private void HandleRelicRemoved(RelicData relic)
    {
        currentRelics.Remove(relic);
        foreach (var effect in relic.relicEffets) effect.Unregister(_ctx);
    }
}
