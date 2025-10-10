using UnityEngine;

public class HealingFountain : MonoBehaviour
{
    [SerializeField] private int _healAmount;

    private void OnMouseDown()
    {
        HealPlayer(_healAmount);
    }

    private void HealPlayer(int amount)
    {
        PlayerManager.Instance.HealthChange(amount, true);
    }
}
