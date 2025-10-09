using UnityEngine;

public class HintAnimationLogic : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private BoxCollider _boxCollider;
    public bool IsShown { get; private set; } = false;
    private int _balance = 2;

    private void OnMouseDown()
    {
        if (RoundManager.Instance.InUI) return;

        if (IsShown)
        {
            HideHint();
            IsShown = false;
            return;
        }

        ShowHint();
       IsShown = true;
     
    }

    private void ShowHint()
    {
        _boxCollider.size = new Vector3(_boxCollider.size.x * _balance, _boxCollider.size.y * _balance, _boxCollider.size.z);

        CamMovementManager.Instance.CheckCameraPosition();

        if (CamMovementManager.Instance.IsFirstSet) 
            _animator.SetTrigger("ShowFirst");
        else
            _animator.SetTrigger("ShowSecond");

    }

    private void HideHint()
    {
        _boxCollider.size = new Vector3(_boxCollider.size.x / _balance, _boxCollider.size.y / _balance, _boxCollider.size.z);
        if (CamMovementManager.Instance.IsFirstSet)
            _animator.SetTrigger("HideFirst");
        else
            _animator.SetTrigger("HideSecond");

    }


}
