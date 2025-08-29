using System;
using System.Collections.Generic;
using UnityEngine;

//to be improved
public class CamMovementManager : MonoBehaviour
{
    public static CamMovementManager Instance { get; private set; }

    //mb incapsulate more later 
    public bool inProgress { get; set; } = false;

    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private CameraAnimation _cameraAnimation;
    [SerializeField] private float _duration;
    [SerializeField] private List<ClassForDictionary<string, Transform>> _dictList;
    private Dictionary<string, Transform> _camPositionsFirstSet = new();
    private Dictionary<string, Transform> _camPositionsSecondSet = new();
    private Dictionary<string, Transform> _currentCamPositionsSet = new();
    private bool _isFirstSet = true;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        _camPositionsFirstSet.Clear();
        _camPositionsSecondSet.Clear();
        for (int i = 0, divider = 0; i < _dictList.Count; i++, divider++)
        {
            if(divider < 3)
                _camPositionsFirstSet.Add(_dictList[i].key, _dictList[i].value);
            else
                _camPositionsSecondSet.Add(_dictList[i].key, _dictList[i].value);

        }
        _currentCamPositionsSet = _isFirstSet ? _camPositionsFirstSet : _camPositionsSecondSet;
    }

    void Update()
    {     
        if (Input.anyKey)
        {
            if (inProgress) return;

            CheckKeyAndActivateAnimation();
            return;
        }

        else if (_cameraTransform.rotation != _currentCamPositionsSet["Main"].rotation) //?
        {
            _currentCamPositionsSet = _isFirstSet ? _camPositionsFirstSet : _camPositionsSecondSet;
            _cameraAnimation.ChangeCameraPlacement(target: _currentCamPositionsSet["Main"], duration: _duration);
            inProgress = false;
            //temp
            UIManager.Instance.TurnRoundOn();
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
                        if (!_isFirstSet)
                        {
                            _cameraAnimation.ChangeCameraPlacement(target: _camPositionsFirstSet["Main"], duration: _duration);
                            inProgress = false;
                            _isFirstSet = true;
                        }
                        break;

                    case KeyCode.A:
                        if(_isFirstSet)
                          _cameraAnimation.ChangeCameraPlacement(target: _camPositionsFirstSet["A"], duration: _duration);
                        else
                          _cameraAnimation.ChangeCameraPlacement(target: _camPositionsSecondSet["A"], duration: _duration);
                        break;

                    case KeyCode.S:
                        if (_isFirstSet)
                        {
                            _cameraAnimation.ChangeCameraPlacement(target: _camPositionsSecondSet["Main"], duration: _duration);
                            inProgress = false;
                            _isFirstSet = false;
                        }
                        else
                            _cameraAnimation.ChangeCameraPlacement(target: _camPositionsSecondSet["S"], duration: _duration);
                        break;

                    case KeyCode.D:
                        if (_isFirstSet)
                            _cameraAnimation.ChangeCameraPlacement(target: _camPositionsFirstSet["D"], duration: _duration);
                        else
                            _cameraAnimation.ChangeCameraPlacement(target: _camPositionsSecondSet["D"], duration: _duration);
                        break;

                    default:  break;
                }
                break; 
            }
        }
    }

    //to be improved
    public void ChangeCameraPlacement(string targetKey)
    {
        //_cameraAnimation.ChangeCameraPlacement(target: _camPositionsFirstSet[targetKey], duration: _duration);
        //_isFirstSet = true;
        //temp
        //UIManager.Instance.TurnRoundOff();
    }   
}
