using System.Collections.Generic;
using UnityEngine;

public class AthenaAnimationLogic : MonoBehaviour
{
    [SerializeField] private float _changeLimit = 2f;
    [SerializeField] private Animator _animator;
    [SerializeField] private List<string> _idleOptions;
    [SerializeField] private string _roundFinishOption;
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
        if(CombatManager.Instance.CurrentNodeType != MapSystem.NodeType.Boss) return;

        _animator.SetTrigger(_roundFinishOption);
        _currentTime = 0f;
    }

}
