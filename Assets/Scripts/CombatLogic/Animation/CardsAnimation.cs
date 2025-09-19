using System.Collections;
using UnityEngine;

public class CardsAnimation : MonoBehaviour
{
    [SerializeField] protected Transform _removedCardPosition;
    [SerializeField] private float _attackDuration = 1f;
    [SerializeField] private float _removalDuration = 1f;
    private float _damageDuration = 0.2f;
    private bool _animatedDamage = false;
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

        cards[0].GetPositionAndRotation(out Vector3 startPos, out Quaternion startRot);
        cards[0].SetParent(null);
        Vector3 startPos1 = cards.Length > 1 ? cards[1].position : Vector3.zero;
        Vector3 startPos2 = cards.Length > 1 ? cards[2].position : Vector3.zero;
        Quaternion startRot1 = cards.Length > 1 ? cards[1].rotation : Quaternion.identity;
        Quaternion startRot2 = cards.Length > 1 ? cards[2].rotation : Quaternion.identity;

        if (cards.Length > 1)
        {
            cards[1].SetParent(null);
            cards[2].SetParent(null);
        }

        float elapsed = 0f;

        while (elapsed < _attackDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _attackDuration);
            float curveT = _curve.Evaluate(t);
            float arc = Mathf.Sin(t * Mathf.PI) * _height;

            Vector3 straightPos = Vector3.Lerp(startPos, attackPosition.position, curveT);
            Vector3 straightPos1 = Vector3.Lerp(startPos1, attackPosition.position, curveT);
            Vector3 straightPos2 = Vector3.Lerp(startPos2, attackPosition.position, curveT);

            Vector3 arcPos = new(straightPos.x, straightPos.y + arc, straightPos.z);
            Vector3 arcPos1 = new(straightPos1.x, straightPos1.y + arc, straightPos1.z);
            Vector3 arcPos2 = new(straightPos2.x, straightPos2.y + arc, straightPos2.z);

            Quaternion rot = Quaternion.Slerp(startRot, _attackRotation, curveT);
            Quaternion rot1 = Quaternion.Slerp(startRot1, _attackRotation, curveT);
            Quaternion rot2 = Quaternion.Slerp(startRot2, _attackRotation, curveT);

            cards[0].SetPositionAndRotation(arcPos, rot);
            if (cards.Length > 1)
            {
                cards[1].SetPositionAndRotation(arcPos1, rot1);
                cards[2].SetPositionAndRotation(arcPos2, rot2);
            }
            yield return null;
        }

        if (isPlayer)
        {
            StartCoroutine(AnimatedDamage(attackPosition, key));
            _animatedDamage = true;
        }

        elapsed = 0f;

        while (elapsed < _attackDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _attackDuration);
            float curveT = _curve.Evaluate(t);
            float arc = Mathf.Sin(t * Mathf.PI) * _height;

            Vector3 straightPos = Vector3.Lerp(attackPosition.position, startPos, curveT);
            Vector3 straightPos1 = Vector3.Lerp(attackPosition.position, startPos1, curveT);
            Vector3 straightPos2 = Vector3.Lerp(attackPosition.position, startPos2, curveT);

            Vector3 arcPos = new(straightPos.x, straightPos.y + arc, straightPos.z);
            Vector3 arcPos1 = new(straightPos1.x, straightPos1.y + arc, straightPos1.z);
            Vector3 arcPos2 = new(straightPos2.x, straightPos2.y + arc, straightPos2.z);

            Quaternion rot = Quaternion.Slerp(_attackRotation, startRot, curveT);
            Quaternion rot1 = Quaternion.Slerp(_attackRotation, startRot1, curveT);
            Quaternion rot2 = Quaternion.Slerp(_attackRotation, startRot2, curveT);

            cards[0].SetPositionAndRotation(arcPos, rot);
            if (cards.Length > 1)
            {
                cards[1].SetPositionAndRotation(arcPos1, rot);
                cards[2].SetPositionAndRotation(arcPos2, rot);
            }

            yield return null;
        }

        while (_animatedDamage)
            yield return null;

        IsAnimating = false;
        RemoveCard(cards);
    }

    private IEnumerator AnimatedDamage(Transform parent, int key)
    {
        parent.gameObject.GetComponent<CardHolderAnimation>().DisableAnimations();

        GameObject card1 = EnemyCardManager.Instance.CardsInGame[key][0];
        card1.transform.SetParent(null);
        GameObject card2 = null;
        GameObject card3 = null;
        if (EnemyCardManager.Instance.CardsInGame[key].Count > 1)
        {
            card2 = EnemyCardManager.Instance.CardsInGame[key][1];
            card2.transform.SetParent(null);
            card3 = EnemyCardManager.Instance.CardsInGame[key][2];
            card3.transform.SetParent(null);
        }

        Vector3 startPos = card1.transform.position;
        Vector3 endPos = startPos + new Vector3(0f, 0.01f, 0.01f);
        Vector3 startPos1 = card2 ? card2.transform.position : Vector3.zero;
        Vector3 endPos1 = startPos1 + new Vector3(0f, 0.01f, 0.01f);
        Vector3 startPos2 = card3 ? card3.transform.position : Vector3.zero;
        Vector3 endPos2 = startPos2 + new Vector3(0f, 0.01f, 0.01f);

        float elapsed = 0f;

        while (elapsed < _damageDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / _attackDuration);
            card1.transform.position = Vector3.Lerp(startPos, endPos, elapsed / _attackDuration);
            if (EnemyCardManager.Instance.CardsInGame[key].Count > 1)
            {
                card2.transform.position = Vector3.Lerp(startPos1, endPos1, elapsed / _attackDuration);
                card3.transform.position = Vector3.Lerp(startPos2, endPos2, elapsed / _attackDuration);
            }

            yield return null;
        }

        elapsed = 0f;

        while (elapsed < _damageDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / _attackDuration);
            card1.transform.position = Vector3.Lerp(endPos, startPos, elapsed / _attackDuration);
            if (EnemyCardManager.Instance.CardsInGame[key].Count > 1)
            {
                card2.transform.position = Vector3.Lerp(endPos1, startPos1, elapsed / _attackDuration);
                card3.transform.position = Vector3.Lerp(endPos2, startPos2, elapsed / _attackDuration);
            }
            yield return null;
        }
            
        card1.transform.position = startPos;
        if (EnemyCardManager.Instance.CardsInGame[key].Count > 1)
        {
            card2.transform.position = startPos1;
            card3.transform.position = startPos2;
        }

        _animatedDamage = false;
    }

    public void RemoveCard(Transform[] cards) { StartCoroutine(AnimatedRemoval(cards)); }
    
    protected IEnumerator AnimatedRemoval(Transform[] cards)
    {
        while (IsAnimating) yield return null;

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

