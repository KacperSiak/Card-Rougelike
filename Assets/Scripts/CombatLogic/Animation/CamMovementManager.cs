using System;
using System.Collections.Generic;
using UnityEngine;

public class CamMovementManager : MonoBehaviour
{
    public static CamMovementManager Instance { get; private set; }

    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private CameraAnimation _cameraAnimation;
    [SerializeField] private float _duration;
    [SerializeField] private List<ClassForDictionary<string, Transform>> _dictList;
    private Dictionary<string, Transform> _camPositions = new();
    private bool _inProgress = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        _camPositions.Clear();
        foreach(var k in _dictList)
        {
            _camPositions.Add(k.key, k.value);
        }
    }

    void Update()
    {
        if (Input.anyKey)
        {
            if (_inProgress)
                return;

            CheckKeyAndActivateAnimation();
            _inProgress = true;
            return;
        }

        else if (_cameraTransform.rotation != _camPositions["Main"].rotation)
        {
            _inProgress = false;
            _cameraAnimation.ChangeCameraPlacement(target: _camPositions["Main"], duration: _duration);
        }

    }

    //to be improved 
    private void CheckKeyAndActivateAnimation()
    {
        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(key))
            {
                switch (key)
                {
                    case KeyCode.W:
                        _cameraAnimation.ChangeCameraPlacement(target: _camPositions["W"], duration: _duration);
                        break;

                    case KeyCode.A:
                        _cameraAnimation.ChangeCameraPlacement(target: _camPositions["A"], duration: _duration);
                        break;

                    case KeyCode.S:
                        _cameraAnimation.ChangeCameraPlacement(target: _camPositions["S"], duration: _duration);
                        break;

                    case KeyCode.D:
                        _cameraAnimation.ChangeCameraPlacement(target: _camPositions["D"], duration: _duration);
                        break;

                    default:  break;
                }
                break; 
            }
        }
    }
}
