using System.Collections;
using UnityEngine;

public class EnemyCardAnimation : MonoBehaviour
{
    [SerializeField] private Transform _removedCardPosition;
    [SerializeField] private float _placementDuration = 1f;
    [SerializeField] private float _removalDuration = 1f;
    [SerializeField] private float _height = 0.4f;
    [SerializeField] private AnimationCurve _curve;

    public void PlaceCards(Transform parentPosition, int amount, int key)
    {
        StartCoroutine(AnimatedPlacement(parentPosition, amount, key));
    }

    public void RemoveCard(Transform[] cards)
    {
        StartCoroutine(AnimatedRemoval(cards));
    }

    private IEnumerator AnimatedRemoval(Transform[] cards)
    {
        foreach (var c in cards)
        {
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

    private IEnumerator AnimatedPlacement(Transform parentPosition, int amount, int key)
    {
        Vector3 _finalSpawnPosition = (amount == 1) ? parentPosition.position : parentPosition.position - new Vector3(0.11f, 0, 0);

        for (int i = 0; i < amount; i++)
        {
            GameObject cardObject = EnemyCardManager.Instance.CardsInGame[key][i];
            cardObject.SetActive(true);
            cardObject.transform.GetPositionAndRotation(out Vector3 startPos, out Quaternion startRot);
            cardObject.transform.localScale /= 1.7f;

            float elapsed = 0f;

            while (elapsed < _placementDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / _placementDuration);
                float curveT = _curve.Evaluate(t);

                Vector3 straightPos = Vector3.Lerp(startPos, _finalSpawnPosition, curveT);

                float arc = Mathf.Sin(t * Mathf.PI) * _height;

                Vector3 arcPos = new (straightPos.x, straightPos.y + arc, straightPos.z);

                Quaternion rot = Quaternion.Slerp(startRot, parentPosition.transform.rotation, curveT);

                cardObject.transform.SetPositionAndRotation(arcPos, rot);

                yield return null;

            }

            cardObject.transform.SetPositionAndRotation(_finalSpawnPosition, parentPosition.transform.rotation);
            _finalSpawnPosition += new Vector3(0.11f, 0, 0);
        }
    }
}
