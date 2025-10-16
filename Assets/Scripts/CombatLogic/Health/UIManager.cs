using System.Collections;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Health")]
    [SerializeField] private TextMeshProUGUI _playerHPText; 
    [SerializeField] private TextMeshProUGUI _enemyHPText;

    [Header("Damage")]
    [SerializeField] private TextMeshProUGUI[] _playerHealthDamage;
    [SerializeField] private TextMeshProUGUI[] _enemyHealthDamage;

    [SerializeField] private TextMeshProUGUI _finalResultText; // temp to mark the end of combat

    private float duration = 0.8f;
    private Vector2 _damageOffset = new Vector2(25f, 30f);
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
    }

    public void ShowPlayerDamage(int damage)
    {
        if(!CamMovementManager.Instance.IsFirstSet)
            StartCoroutine(ShowDamage(damage, _playerHealthDamage));
    }

    public void ShowEnemyDamage(int damage)
    {
        if (!CamMovementManager.Instance.IsFirstSet)
            StartCoroutine(ShowDamage(damage, _enemyHealthDamage));
    }

    private IEnumerator ShowDamage(int damage, TextMeshProUGUI[] text)
    { 
        int index = Random.Range(0, text.Length);

        if (text[index].IsActive())
        {
            StartCoroutine(ShowDamage(damage, text));
            yield break;
        } 

        text[index].text = $"-{damage}";
        text[index].gameObject.SetActive(true);
        Vector2 startPos = text[index].transform.position;
        Vector2 endPos = startPos + _damageOffset; 
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            text[index].transform.position = Vector2.Lerp(startPos, endPos, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        text[index].gameObject.SetActive(false);
        text[index].transform.position = startPos;
    }
}
