using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

public class HealingManager : MonoBehaviour
{
    [SerializeField] private int _healAmount;
    private bool _healEnabled;

    private void Awake()
    {
        _healEnabled = true;
    }
    private void OnMouseDown()
    {
        Heal();
    }

    private void Heal()
    {
        if (_healEnabled)
        {
            Debug.Log($"Healuje cie za {_healAmount}");
            // PlayerOverlay.Instance.AddHealth(_healAmount);
            _healEnabled = false;
        }
        
    }
}
