using System.Collections;
using UnityEngine;

public class CameraAnimation : MonoBehaviour
{
    private Transform _targetTransform;
    private float _duration;

    public void ChangeCameraPlacement(Transform target, float duration)
    { 
        _targetTransform = target;
        _duration = duration;
        StartCoroutine(MoveToTarget());
    }

    public IEnumerator MoveToTarget()
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        float elapsed = 0f;

        while (elapsed < _duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / _duration);
            transform.position = Vector3.Lerp(startPos, _targetTransform.position, t);
            transform.rotation = Quaternion.Slerp(startRot, _targetTransform.rotation, t);
            yield return null;
        }

        transform.position = _targetTransform.position;
        transform.rotation = _targetTransform.rotation;
    }

}
