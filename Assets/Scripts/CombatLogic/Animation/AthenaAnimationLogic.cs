using System.Collections.Generic;
using UnityEngine;

public class AthenaAnimationLogic : MonoBehaviour
{
    [SerializeField] private float _changeLimit = 2f;
    [SerializeField] private Animator _animator;
    [SerializeField] private List<string> _idleOptions;
    [SerializeField] private List<string> _roundFinishOptions;
    private float _currentTime = 0f;

    void Update()
    {
        if (_currentTime < _changeLimit)
        {
            _currentTime += Time.deltaTime;
        }
        else
        {
            _animator.SetTrigger(_idleOptions[Random.Range(0, _idleOptions.Count)]);
            _currentTime = 0f;
        }
    }

    public void RoundFinishAnimation()
    {
        _animator.SetTrigger(_roundFinishOptions[Random.Range(0, _roundFinishOptions.Count)]);
        _currentTime = 0f;
    }

}
