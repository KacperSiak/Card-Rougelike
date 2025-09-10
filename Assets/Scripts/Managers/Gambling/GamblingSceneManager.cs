using System;
using UnityEngine;

public class GamblingSceneManager : PersistenSingleton<GamblingSceneManager>
{
    [SerializeField]
    private ChestOpen chest;
    private GamblingManager manager;

    public Action OnRewardGranted;

    public bool canRoll;
    void Start()
    {
        canRoll = true;
        manager = GamblingManager.Instance;
    }

    private void OnEnable() => OnRewardGranted += ResetState;
    private void OnDisable() => OnRewardGranted -= ResetState;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Backspace))
            ResetState();
    }

    public void ResetState()
    { 
        manager.HideParticles();
        chest.ResetState();
        canRoll = true;
    }
}
