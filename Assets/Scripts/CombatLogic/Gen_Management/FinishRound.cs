using UnityEngine;

public class FinishRound : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (DiscardManager.Instance.InUI) return;
        
        RoundAnimation.Instance.Neutral();
        RoundManager.Instance.EndRound();
    }

}
