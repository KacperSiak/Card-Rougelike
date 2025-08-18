using UnityEngine;
using UnityEngine.UI;

public class HealingManager : MonoBehaviour
{
    [SerializeField] private Button _healButton;
    [SerializeField] private Button _backButton;

    private void Awake()
    {
        _healButton.onClick.AddListener(Heal);
    }

    private void Heal()
    {
        int healAmount = 50;
        PlayerOverlay.instance.AddHealth(healAmount);
        _healButton.onClick.RemoveAllListeners();
    }
}
