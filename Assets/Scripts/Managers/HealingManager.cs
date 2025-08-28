using UnityEngine;
using UnityEngine.UI;

public class HealingManager : MonoBehaviour
{
    [SerializeField] private Button _healButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private int healAmount;

    private void Awake()
    {
        _healButton.onClick.AddListener(Heal);
    }

    private void Heal()
    {
        PlayerOverlay.Instance.AddHealth(healAmount);
        _healButton.onClick.RemoveAllListeners();
    }
}
