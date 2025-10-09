using System.Collections;
using TMPro;
using UnityEngine;

public class RewardPanelLogic : MonoBehaviour
{
    [SerializeField] private GameObject _pressAnyButton;
    [SerializeField] private float _delay = 1f;
    private bool _isShown = false;

    public void ShowReward()
    { 
        gameObject.SetActive(true);
        RoundManager.Instance.InUI = true;
        StartCoroutine(DelayedInfo());
    }

    private IEnumerator DelayedInfo()
    {
        yield return new WaitForSeconds(1f);
        _pressAnyButton.SetActive(true);
        _isShown = true;
    }

    void Update()
    {
        if (Input.anyKey && _isShown)
        {
            gameObject.SetActive(false);
            RoundManager.Instance.InUI = false;
            RoundManager.Instance.FinishCombat();
        }
    }
}
