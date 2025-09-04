using System.Collections;
using UnityEngine;

public class CardHolderAnimation : MonoBehaviour
{
    [SerializeField] private float _duration = 0.1f;
    [SerializeField] private Vector3 _idleV;
    private Vector3 _objectPosition;
    private Vector3 _idlePosition;
    private float _elapsedTime;
    private int interval = 0;
    private bool _isPlayerHand = false;

    private void Awake()
    {
        _objectPosition = transform.position;
        _idlePosition = _objectPosition + _idleV;
    }

    private void OnDisable()
    {
        DisableAnimations();
    }

    public void DisableAnimations()
    {
        StopAllCoroutines();

        if(_isPlayerHand && PlayerCardsAnimation.Instance)
           PlayerCardsAnimation.Instance.StopWaveAnimation();

        transform.position = _objectPosition;
        interval = 0;
    }

    public void EnableAnimations(bool isPHand)
    {
       _isPlayerHand = isPHand;
        StartCoroutine(StartAnimation());
        interval = 0;
    }

    private IEnumerator StartAnimation()
    {
        if (_isPlayerHand)
        {
            while (PlayerCardsAnimation.Instance.WaveAnimationActive)
                yield return null;
        }

        _elapsedTime = 0f;

        while (_elapsedTime < _duration)
        { 
            _elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, _elapsedTime / _duration);
            transform.position = Vector3.Lerp(_objectPosition, _idlePosition, t);

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

            yield return null;
        }

        if (_isPlayerHand && ++interval == PlayerCardsAnimation.Instance.Interval && PlayerCardManager.Instance.CardsInHand != null)
        {
            PlayerCardsAnimation.Instance.StartWaveAnimation();
            interval = 0;
        }

        StartCoroutine(StartAnimation());
    }
}
