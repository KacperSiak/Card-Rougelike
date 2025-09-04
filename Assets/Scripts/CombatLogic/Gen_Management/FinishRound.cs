using UnityEngine;

public class FinishRound : MonoBehaviour
{
   [SerializeField] private Animator _animator;

    private void OnMouseDown()
    {
        _animator.SetTrigger("PlayFinish");
        RoundManager.Instance.EndRound();
    }

}
