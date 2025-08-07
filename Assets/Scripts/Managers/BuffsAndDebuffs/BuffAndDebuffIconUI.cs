using UnityEngine;
using TMPro;

public class BuffAndDebuffIconUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _turnsleft;
    private int _fightsLeftModifier;
    private int _turnsLeftModifier;

    private void Awake()
    {
        RoundManager.OnCombatEnd += ModifyBuffCounter;
        RoundManager.OnRoundEnd += ModifyDebuffCounter;
    }

    public void BuffIconSetup(PotionData potion)
    {
        _fightsLeftModifier = potion.fightsLeft;
        _turnsleft.text = $"{_fightsLeftModifier}";
    }

    private void ModifyBuffCounter()
    {
        _fightsLeftModifier -= 1;
        _turnsleft.text = $"{_fightsLeftModifier}";
        if (_fightsLeftModifier <= 0)
        {
            Destroy(gameObject);
        }
    }
    
    public void DebuffIconSetup(DebuffData debuff)
    {
        _turnsLeftModifier = debuff.debuffTurns;
        _turnsleft.text = $"{_turnsLeftModifier}";
    }

    private void ModifyDebuffCounter()
    {
        _turnsLeftModifier -= 1;
        _turnsleft.text = $"{_fightsLeftModifier}";
        if (_turnsLeftModifier <= 0)
        {
            Destroy(gameObject);
        }
    }
}
