using System.Collections;
using UnityEngine;

public class RoundAnimation : PersistenSingleton<RoundAnimation>
{
    [SerializeField] private Transform _roundSetter;
    [SerializeField] private float _animationDuration = 0.5f;
    [SerializeField] private RoundEndSFX _sfx;
    private bool _isAnimating = false;

    private void OnEnable()
    {
        RoundManager.OnCombatStart += EnemyRound;
    }

    private void OnDisable()
    {
        RoundManager.OnCombatStart -= EnemyRound;
    }

    public void PlayerRound()
    { 
        StartCoroutine(AnimationBuffer(Quaternion.Euler(110, 0, 180)));
    }

    public void EnemyRound()
    {
        StartCoroutine(AnimationBuffer(Quaternion.Euler(70, 0, 180)));
    }

    public void Neutral()
    {
        StartCoroutine(AnimationBuffer(Quaternion.Euler(90, 0, 180)));
    }

    private IEnumerator AnimationBuffer(Quaternion position)
    { 
        while (_isAnimating)
            yield return null;

        StartCoroutine(Animation(position));
        _sfx.Play();
    }

    private IEnumerator Animation(Quaternion endPosition)
    { 
        _isAnimating = true;
        Quaternion startRotation = _roundSetter.rotation;

        float elapsedTime = 0f;

        while (elapsedTime < _animationDuration)
        {
            elapsedTime += Time.deltaTime;
            _roundSetter.rotation = Quaternion.Slerp(startRotation, endPosition, elapsedTime / _animationDuration);
            yield return null;
        }

        _roundSetter.rotation = endPosition;
        _isAnimating = false;
    }
}
