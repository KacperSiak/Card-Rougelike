using System.Collections;
using UnityEngine;

public class IdleHandAnimationLogic : MonoBehaviour
{
    [SerializeField] private float _duration = 0.1f; 
    [SerializeField] private float _idleValue = 0.1f;
    private Vector3 _objectPosition;
    private Vector3 _idlePosition;
    private float _elapsedTime;

    private void OnEnable()
    {
        RoundManager.OnCombatStart += StartAnimations;
    }

    void StartAnimations()
    {
        _objectPosition = transform.position;
        _idlePosition = new Vector3(_objectPosition.x, _objectPosition.y + _idleValue, _objectPosition.z + +_idleValue);
        StartCoroutine(StartAnimation());
    }

    private void OnDisable()
    {
        RoundManager.OnCombatStart -= StartAnimations;
        StopAllCoroutines();
    }

    private IEnumerator StartAnimation()
    {
        _elapsedTime = 0f;

        while (_elapsedTime < _duration)
        { 
            _elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(_objectPosition, _idlePosition, _elapsedTime / _duration);

            if (PlayerCardManager.Instance.CardsInHand != null)
            { foreach (var card in PlayerCardManager.Instance.CardsInHand)
                {
                    card.GetComponent<PlayerCardLogic>().SetAnimationPlacement(newPosition: card.transform.position, newRotation: card.transform.rotation);
                }
            }
            yield return null;
        }

        StartCoroutine(EndAnimation());

    }

    private IEnumerator EndAnimation()
    {
        _elapsedTime = 0f;

        while (_elapsedTime < _duration)
        {
            _elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(_idlePosition, _objectPosition, _elapsedTime / _duration);
            if (PlayerCardManager.Instance.CardsInHand != null)
            {
                foreach (var card in PlayerCardManager.Instance.CardsInHand)
                {
                    card.GetComponent<PlayerCardLogic>().SetAnimationPlacement(newPosition: card.transform.position, newRotation: card.transform.rotation);
                }
            }

            yield return null;
        }

        StartCoroutine(StartAnimation());
    }
}
