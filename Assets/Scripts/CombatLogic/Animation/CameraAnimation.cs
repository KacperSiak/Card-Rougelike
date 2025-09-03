using System.Collections;
using UnityEngine;

public class CameraAnimation : MonoBehaviour
{
    private Transform _targetTransform;
    private float _duration;
    private bool _isPlaying = false;

    public void ChangeCameraPlacement(Transform target, float duration, bool isChanging)
    {
        if (_isPlaying) return;
        
        _targetTransform = target;
        _duration = duration;
        StartCoroutine(MoveToTarget(isChanging));
        _isPlaying = true;
        CamMovementManager.Instance.InProgress = true;
        //temp
        UIManager.Instance.TurnRoundOff();
    }

    private IEnumerator MoveToTarget(bool isChanging)
    {
        transform.GetPositionAndRotation(out Vector3 startPos, out Quaternion startRot);
        float elapsed = 0f;

        while (elapsed < _duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / _duration);
            transform.SetPositionAndRotation(Vector3.Lerp(startPos, _targetTransform.position, t), Quaternion.Slerp(startRot, _targetTransform.rotation, t));
            yield return null;
        }

        transform.SetPositionAndRotation(_targetTransform.position, _targetTransform.rotation);
        _isPlaying = false;
        if (isChanging)
        {
            PlayerCardManager.Instance.SetMainHand();
            PlayerCardManager.Instance.SetCards();
        }
           
    }

}
