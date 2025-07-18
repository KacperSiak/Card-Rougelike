using System;
using UnityEngine;
using UnityEngine.UI;

public class FinishRoundBtn : MonoBehaviour
{
    private Button _button;
    [SerializeField] RoundManager _roundManager;

    void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnFinishRoundClicked);
    }

    private void OnFinishRoundClicked()
    {
        _roundManager.EndRound();
    }

}
