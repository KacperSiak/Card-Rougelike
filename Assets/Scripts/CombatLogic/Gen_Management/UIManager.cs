using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI _playerHPText; 
    [SerializeField] private TextMeshProUGUI _enemyHPText; 
    [SerializeField] private TextMeshProUGUI _finalResultText; // temp to mark the end of combat
    [SerializeField] private List<TextMeshProUGUI> _roundsText;
    [SerializeField] private EnemyAttackManager _enemyAttackManager;

    private string _winner;

    public string Winner
    {
        get { return _winner; }
        set { _winner = value; }
    }


    void Awake()
    {
        if (Instance == null) Instance = this;

        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        RoundManager.OnHPChanged += SetHealthUI;
        RoundManager.OnCombatEnd += SetupCombatEndUI;
        RoundManager.OnCombatStart += StartCombat;
    }


    private void OnDisable()
    {
        RoundManager.OnHPChanged -= SetHealthUI;
        RoundManager.OnCombatEnd -= SetupCombatEndUI;
        RoundManager.OnCombatStart -= StartCombat;
    }

    public void StartCombat()
    {
        SetHealthUI();
    }

    private void SetHealthUI()
    {
        _playerHPText.text = $"Player HP: {RoundManager.Instance.PlayerHP}";
        _enemyHPText.text = $"Enemy HP: {RoundManager.Instance.EnemyHP}";
    }

    public void SetRoundForEnemyUI(int key)
    {
        if (!_enemyAttackManager.RoundsForCards.ContainsKey(key) || _enemyAttackManager.RoundsForCards[key] == 0)
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
        _finalResultText.text = $"Winner: {_winner}";
        _finalResultText.gameObject.SetActive(true);
    }

    //temp
    public void TurnRoundOn()
    {
        foreach (var item in _roundsText)
        {
            item.gameObject.SetActive(true);
        }
    }

    public void TurnRoundOff()
    {
        foreach (var item in _roundsText)
        {
            item.gameObject.SetActive(false);
        }
    }
}
