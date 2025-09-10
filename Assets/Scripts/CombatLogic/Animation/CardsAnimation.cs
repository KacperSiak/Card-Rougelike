using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CardsAnimation : MonoBehaviour
{
    [SerializeField] protected Transform _removedCardPosition;
    [SerializeField] private float _attackDuration = 1f;
    [SerializeField] protected Quaternion _attackRotation;
    [SerializeField] private float _removalDuration = 1f;
    [SerializeField] protected float _height = 0.4f;
    [SerializeField] protected AnimationCurve _curve;
    public bool IsAnimating { get; protected set; } = false;

    public void Attack(Transform attackPosition, Transform[] cards)
    {
        StartCoroutine(AnimatedAttack(attackPosition, cards));
    }

    private IEnumerator AnimatedAttack(Transform attackPosition, Transform[] cards)
    {
        IsAnimating = true;

        foreach (var c in cards)
        {
            c.transform.GetPositionAndRotation(out Vector3 startPos, out Quaternion startRot);

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

            if (attackPosition.childCount != 0)
            { 
               //foreach (Transform child in attackPosition)
               //     child.AddComponent<Animator>().SetTrigger("Attack");
            }

            //cardObject.transform.SetParent(attackPosition, true);
        }

        IsAnimating = false;
        //EnemyCardManager.Instance.ControlIdleAnimation();
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

