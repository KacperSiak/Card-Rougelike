using UnityEngine;
using System.Collections;

public class ChestOpen : MonoBehaviour
{
    [SerializeField] private Vector3 _startPos;
    [SerializeField] private Vector3 _endPos;
    [SerializeField] private float _duration;

    private float _elapsed = 0f;
    private Animator _anim;
    private bool _canOpen;
    private bool _opening;
    
    private void Awake()
    {
        _anim = GetComponentInChildren<Animator>();
        
    }

    private void Update()
    {
        if (_opening && _elapsed < _duration)
        {
            _elapsed += Time.deltaTime;
            float t = _elapsed / _duration;
            transform.position = Vector3.Lerp(_startPos, _endPos, t);

            if (_elapsed >= _duration)
            {
                _opening = false;
                _canOpen = true;
            }
        }
    }

    private void OnMouseDown()
    {
        if (_canOpen)
        {
            _canOpen = false;
            _anim.SetTrigger("ChestOpen");
            GamblingManager.Instance.Roll();
        }
    }

    public void ChestOpening()
    {
        _opening = true;
    }
}
