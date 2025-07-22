using System;
using UnityEngine;
using UnityEngine.UI;

public class FinishRoundBtn : MonoBehaviour
{
    private Button _button;

    void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnFinishRoundClicked);
    }

    private void OnFinishRoundClicked()
    {
        RoundManager.Instance.EndRound();
    }

}
