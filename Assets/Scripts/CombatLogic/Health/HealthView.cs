using UnityEngine;

public class HealthView : MonoBehaviour
{
    [SerializeField] private GameObject _healthUI;

    private void OnMouseEnter()
    {
        _healthUI.SetActive(true);
    }

    private void OnMouseExit()
    {
        _healthUI.SetActive(false);
    }
}
