using UnityEngine;

public class DiceLogic : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private MeshRenderer _meshRenderer;

    public void UpdateRounds(int key)
    {
        _animator.SetTrigger("Roll");
        MaterialSetupManager.Instance.SetupVisual(_meshRenderer, CombatManager.Instance._diceDictionary[key].value, CombatManager.Instance._diceDictionary[key].key);
    }
    
}
