using System.Collections;
using UnityEngine;

public class CardsAnimation : MonoBehaviour
{
    [SerializeField] protected Transform _removedCardPosition;
    [SerializeField] private float _removalDuration = 1f;
    [SerializeField] protected float _height = 0.4f;
    [SerializeField] protected AnimationCurve _curve;

    public void RemoveCard(Transform[] cards)
    {
        StartCoroutine(AnimatedRemoval(cards));
    }

    protected IEnumerator AnimatedRemoval(Transform[] cards)
    {
        foreach (var c in cards)
        {
            c.SetParent(null);
            c.GetPositionAndRotation(out Vector3 startPos, out Quaternion startRot);
            float elapsed = 0f;

            while (elapsed < _removalDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / _removalDuration);
                float curveT = _curve.Evaluate(t);

                Vector3 straightPos = Vector3.Lerp(startPos, _removedCardPosition.position, curveT);

                float arc = Mathf.Sin(t * Mathf.PI) * _height;

                Vector3 arcPos = new(straightPos.x, straightPos.y + arc, straightPos.z);

                Quaternion rot = Quaternion.Slerp(startRot, _removedCardPosition.rotation, curveT);

                c.SetPositionAndRotation(arcPos, rot);

                yield return null;
            }

            ObjPoolManager.Instance.ReturnCard(c.gameObject);
        }
    }

}

