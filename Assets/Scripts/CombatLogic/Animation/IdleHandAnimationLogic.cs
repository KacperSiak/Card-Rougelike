using System.Collections;
using UnityEngine;

public class IdleHandAnimationLogic : MonoBehaviour
{
    [SerializeField] private float _duration = 0.1f; 
    [SerializeField] private float _idleValue = 0.1f;
    private Vector3 _objectPosition;
    private Vector3 _idlePosition;
    private float _elapsedTime;
    private int interval = 0;

    private void OnEnable()
    {
        RoundManager.OnCombatStart += StartAnimations;
    }

    private void OnDisable()
    {
        RoundManager.OnCombatStart -= StartAnimations;
        StopAllCoroutines();
    }

    void StartAnimations()
    {
        _objectPosition = transform.position;
        _idlePosition = new Vector3(_objectPosition.x, _objectPosition.y + _idleValue, _objectPosition.z + +_idleValue);
        StartCoroutine(StartAnimation());
    }

    private IEnumerator StartAnimation()
    {
        while(PlayerCardsAnimation.Instance.WaveAnimationActive)
            yield return null;

        _elapsedTime = 0f;

        while (_elapsedTime < _duration)
        { 
            _elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, _elapsedTime / _duration);
            transform.position = Vector3.Lerp(_objectPosition, _idlePosition, t);

            if (PlayerCardManager.Instance.CardsInHand != null)
            {
                foreach (var card in PlayerCardManager.Instance.CardsInHand)
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

        if (++interval == PlayerCardsAnimation.Instance.Interval && PlayerCardManager.Instance.CardsInHand != null)
        {
            PlayerCardsAnimation.Instance.StartWaveAnimation();
            interval = 0;
        }

        StartCoroutine(StartAnimation());
    }
}
