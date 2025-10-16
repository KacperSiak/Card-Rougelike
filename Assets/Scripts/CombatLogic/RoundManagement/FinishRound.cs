using UnityEngine;

public class FinishRound : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (RoundManager.Instance.InUI) return;
        
        RoundAnimation.Instance.Neutral();
        RoundManager.Instance.EndRound();
    }

}
