using UnityEngine;

public class CameraFly : MonoBehaviour
{
    //temp prototype
    [SerializeField] private Transform _cameraTransform;
    //[SerializeField] private Transform _target;
    private Vector3 _tempPosition = new Vector3(-0.011f, 2.8f, -7.16f); 
    private Quaternion _defaultRotation;
    private Vector3 _defaultPosition;

    void Start()
    {
        _defaultRotation = _cameraTransform.rotation;
        _defaultPosition = _cameraTransform.position;
    }

    //temp
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            _cameraTransform.Rotate(40f, 0f, 0f);
            _cameraTransform.position = _tempPosition;
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            _cameraTransform.rotation = _defaultRotation;
            _cameraTransform.position = _defaultPosition;
        }
    }

}