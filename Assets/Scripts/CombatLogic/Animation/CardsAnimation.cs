using System.Collections;
using UnityEngine;

public class CardsAnimation : MonoBehaviour
{
    [SerializeField] protected Transform _removedCardPosition;
    [SerializeField] private float _attackDuration = 1f;
    [SerializeField] private float _removalDuration = 1f;
    [SerializeField] private float _damageDuration = 1f;
    [SerializeField] protected float _height = 0.4f;
    [SerializeField] protected AnimationCurve _curve;
    public bool IsAnimating { get; protected set; } = false;

    public void Attack(Transform attackPosition, Transform[] cards, bool isPlayer, int key)
    {
        StartCoroutine(AnimatedAttack(attackPosition, cards, isPlayer, key));
    }

    private IEnumerator AnimatedAttack(Transform attackPosition, Transform[] cards, bool isPlayer, int key)
    {
        IsAnimating = true;
        Quaternion _attackRotation = isPlayer ? Quaternion.Euler(340f, 0f, 0f) : Quaternion.Euler(-170f, 0f, 0f);

        foreach (var c in cards)
        {
            c.transform.GetPositionAndRotation(out Vector3 startPos, out Quaternion startRot);
            c.transform.SetParent(null);

            float elapsed = 0f;

            while (elapsed < _attackDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / _attackDuration);
                float curveT = _curve.Evaluate(t);

                Vector3 straightPos = Vector3.Lerp(startPos, attackPosition.position, curveT);

                float arc = Mathf.Sin(t * Mathf.PI) * _height;

                Vector3 arcPos = new(straightPos.x, straightPos.y + arc, straightPos.z);

                Quaternion rot = Quaternion.Slerp(startRot, _attackRotation, curveT); //temp

                c.transform.SetPositionAndRotation(arcPos, rot);
                yield return null;
            }

            if (isPlayer)
            {
                StartCoroutine(AnimatedDamage(attackPosition, key));
                yield return new WaitForSeconds(_damageDuration * attackPosition.childCount);
            }

            elapsed = 0f;

            while (elapsed < _attackDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / _attackDuration);
                float curveT = _curve.Evaluate(t);

                Vector3 straightPos = Vector3.Lerp(attackPosition.position, startPos, curveT);

                float arc = Mathf.Sin(t * Mathf.PI) * _height;

                Vector3 arcPos = new(straightPos.x, straightPos.y + arc, straightPos.z);

                Quaternion rot = Quaternion.Slerp(_attackRotation, startRot, curveT); 

                c.transform.SetPositionAndRotation(arcPos, rot);
                yield return null;
            }

            c.transform.SetPositionAndRotation(startPos,startRot);

        }

        IsAnimating = false;
        RemoveCard(cards);
    }

    private IEnumerator AnimatedDamage(Transform parent, int key)
    {
        parent.gameObject.GetComponent<CardHolderAnimation>().DisableAnimations();

        foreach (GameObject card in EnemyCardManager.Instance.CardsInGame[key])
        {
            Vector3 startPos = card.transform.position;
            Vector3 endPos = card.transform.position + new Vector3(0f, 0.1f, 0.1f);
            card.transform.SetParent(null);

            float elapsed = 0f;

            while (elapsed < _attackDuration)
            {
                elapsed += Time.deltaTime;
                card.transform.position = Vector3.Lerp(startPos, endPos, elapsed / _attackDuration);

                yield return null;
            }

            card.transform.position = endPos;
            elapsed = 0f;

            while (elapsed < _attackDuration)
            {
                elapsed += Time.deltaTime;
                card.transform.position = Vector3.Lerp(endPos, startPos, elapsed / _attackDuration);

                yield return null;
            }

            card.transform.position = startPos;
            card.transform.SetParent(parent);
        }

        //parent.gameObject.GetComponent<CardHolderAnimation>().EnableAnimations(isPHand: false);
    }


    public void RemoveCard(Transform[] cards)
    {
        StartCoroutine(AnimatedRemoval(cards));
    }

    protected IEnumerator AnimatedRemoval(Transform[] cards)
    {
        while (IsAnimating)
            yield return null;

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

