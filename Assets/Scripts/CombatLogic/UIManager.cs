using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI _playerHPText; 
    [SerializeField] private TextMeshProUGUI _enemyHPText; 
    [SerializeField] private TextMeshProUGUI _finalResultText;
    [SerializeField] private List<TextMeshProUGUI> _roundsText;
    [SerializeField] private EnemyAttackManager _enemyAttackManager;

    void Awake()
    {
        if (Instance == null) Instance = this;

        else Destroy(gameObject);
    }

    void Start()
    {
        RoundManager.OnCombatEnd += SetupCombatEndUI;
    }

    public void SetHealthUI()
    {
        _playerHPText.text = $"Player HP: {RoundManager.Instance.PlayerHP}";
        _enemyHPText.text = $"Enemy HP: {RoundManager.Instance.EnemyHP}";
    }

    public void SetRoundForEnemyUI(int key)
    {
        if (_enemyAttackManager.RoundsForCards[key] == 0)
        {
            _roundsText[key].text = string.Empty;
            return;
        }

        _roundsText[key].text = $"{_enemyAttackManager.RoundsForCards[key]}";

    }

    private void SetupCombatEndUI()
    {
        foreach (var text in _roundsText)
        {
            text.text = string.Empty;
        }
        _playerHPText.text = string.Empty;
        _enemyHPText.text = string.Empty;

    }

   
}
