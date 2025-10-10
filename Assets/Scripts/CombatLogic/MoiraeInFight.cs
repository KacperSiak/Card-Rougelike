using System.Collections.Generic;
using UnityEngine;

public class MoiraeInFight : MonoBehaviour
{
    [SerializeField] private GameObject _hand1;
    [SerializeField] private GameObject _hand2;
    [SerializeField] private GameObject _temporaryConsumableHand;
    [SerializeField] private GameObject _temporaryCardHand;

    private bool _consumable = false;

    private void OnMouseDown()
    {
        Debug.Log("Klik³em");
        GameObject activeHand = _hand1.activeSelf ? _hand1 : _hand2;
        Debug.Log($"Aktywna rêka: {activeHand.name}");

        GameObject outTemp = _consumable ? _temporaryConsumableHand : _temporaryCardHand;
        GameObject inTemp = _consumable ? _temporaryCardHand : _temporaryConsumableHand;

        Debug.Log($"Idziemy do: {outTemp.name}");
        Debug.Log($"Dostajemy o: {inTemp.name}");

        var children = new List<Transform>();
        foreach (Transform ch in activeHand.transform) children.Add(ch);
        foreach (Transform ch in children) 
        { 
            ch.SetParent(outTemp.transform, false); 
        }


        var coming = new List<Transform>();
        foreach(Transform ch in inTemp.transform) coming.Add(ch);
        foreach (Transform ch in coming)
        {
            ch.SetParent(activeHand.transform, false);
        }

        if (_consumable) _consumable = false;
        else _consumable = true;
    }
}
