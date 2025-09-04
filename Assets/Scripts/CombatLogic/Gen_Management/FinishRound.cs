using UnityEngine;

public class FinishRound : MonoBehaviour
{
   [SerializeField] private Animator _animator;

    private void OnMouseDown()
    {
        if (DiscardManager.Instance.InUI) return;
        
        _animator.SetTrigger("PlayFinish");
        RoundManager.Instance.EndRound();
    }

}
