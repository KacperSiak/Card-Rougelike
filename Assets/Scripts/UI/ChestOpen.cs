using UnityEngine;
using System.Collections;

public class ChestOpen : MonoBehaviour
{
    private Animator _anim;
    private bool _canOpen;
    private bool rewardGranted;

    float timer;
    bool timerStarted;
    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _canOpen = false;
        rewardGranted = false;
    
        timer = 0;
    }

    private void OnMouseDown()
    {
        if (_canOpen)
        {
            _canOpen = false;
            _anim.SetTrigger("ChestOpen");
            timerStarted = true;
        }
        if (rewardGranted)
        {
            GamblingSceneManager.Instance.OnRewardGranted();
        }
    }

    private void Update()
    {
        if (!timerStarted) return;

        timer += Time.deltaTime;
        if(timer >= 4)
        {
            rewardGranted = true;
            timerStarted = false;
            timer = 0;
        }
    }

    public void ResetState()
    {
        _anim.SetTrigger("ChestHide");
        DisableOpening();
        rewardGranted = false;
    }

    public void EnableOpening() => _canOpen = true;
    public void DisableOpening() => _canOpen = false;

    public void RollReward()
    {
        if (GamblingManager.Instance)
            GamblingManager.Instance.Roll();
        else
            Debug.LogWarning("There's no Gambling Manager on this scene");
    }
}
