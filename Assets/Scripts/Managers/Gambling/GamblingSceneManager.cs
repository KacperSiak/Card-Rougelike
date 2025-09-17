using System;
using Systems.SceneManagement;
using TMPro;
using UnityEngine;

public class GamblingSceneManager : PersistenSingleton<GamblingSceneManager>
{
    [SerializeField]
    private ChestOpen chest;
    private GamblingManager manager;

    [SerializeField] private GameObject canvas;
    [SerializeField] private TextMeshProUGUI text;

    public event Action OnRewardGranted;

    public bool canRoll;
    public bool isIdle;
    void Start()
    {
        canRoll = true;
        isIdle = true;
        manager = GamblingManager.Instance;
    }

    private void OnEnable() => OnRewardGranted += ResetState;
    private void OnDisable() => OnRewardGranted -= ResetState;

    public void CompleteTransaction()
    {
        OnRewardGranted.Invoke();
    }

    public void ResetState()
    { 
        manager.HideParticles();
        chest.ResetState();
        HideReward();
        canRoll = true;
        isIdle = true;
    }

    public void ShowReward(string rewardName)
    {
        canvas.SetActive(true);
        text.text = rewardName;
    }

    void ReturnToMap()
    {
        return;
    }

    public void HideReward() => canvas.SetActive(false);
}
