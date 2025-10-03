using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class MoiraeInFight : MonoBehaviour
{
    [SerializeField] private GameObject _hand1;
    [SerializeField] private GameObject _hand2;
    [SerializeField] private GameObject _temporaryConsumableHand;
    [SerializeField] private GameObject _temporaryCardHand;

    [SerializeField] private float _duration;

    private bool _consumable = false;

    private void OnMouseDown()
    {
        GameObject activeHand = _hand1.activeSelf ? _hand1 : _hand2;

        GameObject outTemp = _consumable ? _temporaryConsumableHand : _temporaryCardHand;
        GameObject inTemp = _consumable ? _temporaryCardHand : _temporaryConsumableHand;

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

    //private void AnimateCard(Transform card, Vector3 endPos, Transform newParent, bool reparent)
    //{
    //    StartCoroutine(MoveCards(card, endPos, newParent, reparent));
    //}

    //private System.Collections.IEnumerator MoveCards(Transform card, Vector3 endPos, Transform newParent, bool reparent)
    //{
    //    Vector3 startPos = card.position;
    //    float elapsed = 0;

    //    while (elapsed < _duration)
    //    {
    //        elapsed += Time.deltaTime;
    //        float t = elapsed / _duration;
    //        card.position = Vector3.Lerp(startPos, endPos, t);
    //        yield return null;
    //    }

    //    card.position = endPos;

    //    if (reparent)
    //    {
    //        card.SetParent(newParent, false);
    //    }
    //}
}
