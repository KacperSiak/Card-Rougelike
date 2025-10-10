using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardPanelLogic : MonoBehaviour
{
    [SerializeField] private GameObject _pressAnyButton;
    [SerializeField] private GameObject _bossPanel;
    [SerializeField] private Transform _parent;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private float _delay = 1f;
    private bool _isShown = false;

    public void SetReward(Sprite image, string text)
    { 
        GameObject reward = Instantiate(_prefab, _parent);
        reward.GetComponentInChildren<Image>().sprite = image;
        reward.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }

    public void ShowRewards()
    { 
        gameObject.SetActive(true);
        RoundManager.Instance.InUI = true;
        StartCoroutine(DelayedInfo());
    }

    private IEnumerator DelayedInfo()
    {
        yield return new WaitForSeconds(1f);
        if(RewardManager.Instance.IsBoss) _bossPanel.SetActive(true);
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
