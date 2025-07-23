using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FinishRoundBtn : MonoBehaviour
{
    private Button _button;

    void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnFinishRoundClicked);
        RoundManager.OnCombatEnd += OnCombatEnd;
    }

    private void OnFinishRoundClicked()
    {
        RoundManager.Instance.EndRound();
    }

    private void OnCombatEnd()
    {
        _button.onClick.RemoveAllListeners(); 
    }

}
