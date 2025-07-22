using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ConsumblesMenager : MonoBehaviour
{
    public List<RelicData> currentRelics;
    public RelicUIDisplay relicUI;

    private RelicContext ctx;

    private void Awake()
    {
        RelicEventsSystem.OnRelicAcquired.AddListener(HandleRelicAcquired);
        RelicEventsSystem.OnRelicRemoved.AddListener(HandleRelicRemoved);
    }

    private void Start()
    {
        ctx = new RelicContext { player = PlayerOverlay.instance };
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
            foreach (var effect in relic.relicEffets) effect.Register(ctx);
            relicUI.AddIcon(relic);
        }
    }

    private void HandleRelicRemoved(RelicData relic)
    {
        currentRelics.Remove(relic);
        foreach (var effect in relic.relicEffets) effect.Unregister(ctx);
    }
}
