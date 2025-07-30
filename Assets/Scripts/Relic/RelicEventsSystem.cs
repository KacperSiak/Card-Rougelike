using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public static class RelicEventsSystem 
{
    
    public static UnityEvent<RelicData> OnRelicAcquired = new UnityEvent<RelicData>();
    public static UnityEvent<RelicData> OnRelicRemoved = new UnityEvent<RelicData>();

    public static void RelicAcquired(RelicData relic)
    {
        OnRelicAcquired?.Invoke(relic);
    }

    public static void RelicRemoved(RelicData relic)
    {
        OnRelicRemoved?.Invoke(relic);
    }
}
