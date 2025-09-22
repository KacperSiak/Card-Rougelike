using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI _playerHPText; 
    [SerializeField] private TextMeshProUGUI _enemyHPText; 
    [SerializeField] private TextMeshProUGUI _finalResultText; // temp to mark the end of combat

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
        _playerHPText.text = $"HP: {RoundManager.Instance.PlayerHP}/50"; //temp 50
        _enemyHPText.text = $"HP: {RoundManager.Instance.EnemyHP}/50";
    }

    private void SetupCombatEndUI()
    {
        _playerHPText.text = string.Empty;
        _enemyHPText.text = string.Empty;
        _finalResultText.text = $"Winner: {_winner}";
        //_finalResultText.gameObject.SetActive(true);
    }
}
