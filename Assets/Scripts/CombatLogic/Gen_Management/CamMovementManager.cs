using System;
using System.Collections.Generic;
using UnityEngine;

//to be improved
public class CamMovementManager : PersistenSingleton<CamMovementManager>
{
    //mb incapsulate more later 
    public bool InProgress { get; set; } = false;
    public bool IsFirstSet { get; private set; } = true;

    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private CameraAnimation _cameraAnimation;
    [SerializeField] private float _duration;
    [SerializeField] private List<StructForDictionary<string, Transform>> _dictList;
    private Dictionary<string, Transform> _camPositionsFirstSet = new();
    private Dictionary<string, Transform> _camPositionsSecondSet = new();
    private Dictionary<string, Transform> _currentCamPositionsSet = new();

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
        _currentCamPositionsSet = IsFirstSet ? _camPositionsFirstSet : _camPositionsSecondSet;
    }

    void Update()
    {     
        if (Input.anyKey)
        {
            if (InProgress || DiscardManager.Instance.InUI) return;

            CheckKeyAndActivateAnimation();
            return;
        }

        else if (_cameraTransform.rotation != _currentCamPositionsSet["Main"].rotation) //?
        {
            _currentCamPositionsSet = IsFirstSet ? _camPositionsFirstSet : _camPositionsSecondSet;
            _cameraAnimation.ChangeCameraPlacement(target: _currentCamPositionsSet["Main"], duration: _duration, isChanging: false);
            InProgress = false;
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
                        if (!IsFirstSet)
                        {
                            _cameraAnimation.ChangeCameraPlacement(target: _camPositionsFirstSet["Main"], duration: _duration, isChanging: true);
                            InProgress = false;
                            IsFirstSet = true;
                        }
                        break;

                    case KeyCode.A:
                        if(IsFirstSet)
                          _cameraAnimation.ChangeCameraPlacement(target: _camPositionsFirstSet["A"], duration: _duration, isChanging: false);
                        else
                          _cameraAnimation.ChangeCameraPlacement(target: _camPositionsSecondSet["A"], duration: _duration, isChanging: false);
                        break;

                    case KeyCode.S:
                        if (IsFirstSet)
                        {
                            _cameraAnimation.ChangeCameraPlacement(target: _camPositionsSecondSet["Main"], duration: _duration, isChanging: false);
                            InProgress = false;
                            IsFirstSet = false;
                            //temp
                            PlayerCardManager.Instance.SetMainHand();
                            PlayerCardManager.Instance.SetCards();
                        }
                        else
                            _cameraAnimation.ChangeCameraPlacement(target: _camPositionsSecondSet["S"], duration: _duration, isChanging: false);
                        break;

                    case KeyCode.D:
                        if (IsFirstSet)
                            _cameraAnimation.ChangeCameraPlacement(target: _camPositionsFirstSet["D"], duration: _duration, isChanging: false);
                        else
                            _cameraAnimation.ChangeCameraPlacement(target: _camPositionsSecondSet["D"], duration: _duration, isChanging: false);
                        break;

                    default:  break;
                }
                break; 
            }
        }
    }

    //to be improved
    public void ChangeCameraPlacementOnSelected()
    {
        _cameraAnimation.ChangeCameraPlacement(target: _camPositionsFirstSet["Main"], duration: _duration, isChanging: true);
        InProgress = false;
        IsFirstSet = true;
    }   
}
