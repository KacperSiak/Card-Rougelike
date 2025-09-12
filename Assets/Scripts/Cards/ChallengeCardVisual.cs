using Ink.Parsed;
using UnityEngine;

public class ChallengeCardVisual : MonoBehaviour
{
    [SerializeField] private MeshRenderer _front;
    [SerializeField] private MeshRenderer _back;

    [SerializeField] private Texture2D _easy;
    [SerializeField] private Texture2D _normal;
    [SerializeField] private Texture2D _hard;

    [SerializeField] private Collider _frontColl;
    [SerializeField] private Collider _backColl;

    [Header ("Movement")]
    [SerializeField] private Vector3 _startPos;
    [SerializeField] private Vector3 _endPos;
    [SerializeField] private float _moveDuration;

    [Header("Rotation")]
    [SerializeField] private Vector3 _startRot;
    [SerializeField] private Vector3 _endRot;
    [SerializeField] private float _rotDuration;

    private bool _isMoving,  _isRotating; 
    private float _time;
    private Quaternion _qStart, _qEnd;

    private void OnEnable()
    {
        _frontColl.enabled = false;
        _backColl.enabled = false;
    }

    public void SetupChalss(int col, int row)
    {
        MaterialSetupManager.Instance.SetupVisual(_front, col, row);
    }

    public void SetupBack(int number)
    {
        Texture2D chosen = number switch
        {
            0 => _easy,
            1 => _normal,
            2 => _hard,
            _ => _easy
        };
        _back.material.mainTexture = chosen;
    }

    public void StartAnims()
    {
        _isMoving = true;
        _qStart = Quaternion.Euler(_startRot);
        _qEnd = Quaternion.Euler(_endRot);

        _time = 0f;

        transform.SetPositionAndRotation(_startPos, _qStart);
    }

    private void Update()
    {
        if (_isMoving)
        {
            _time += Time.deltaTime;

            float u = Mathf.Clamp01(_time / _moveDuration);

            transform.SetPositionAndRotation(Vector3.Lerp(_startPos, _endPos, u), _qStart);

            if (u >= 1f)
            {
                _isMoving = false;
                _time = 0f;
                _isRotating = true;
            }
        }
        if (_isRotating)
        {
            _time += Time.deltaTime;
            float u2 = Mathf.Clamp01(_time / _rotDuration);

            transform.position = _endPos;
            transform.rotation = Quaternion.Slerp(_qStart, _qEnd, u2);

            if (u2 >= 1f)
            {
                _isRotating = false;
                _time = 0f;
                _frontColl.enabled = true;
            }
        }
    }
}
