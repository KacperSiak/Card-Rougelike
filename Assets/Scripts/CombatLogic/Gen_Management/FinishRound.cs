using UnityEngine;

public class FinishRound : MonoBehaviour
{
    private void OnMouseDown()
    {
        RoundManager.Instance.EndRound();
    }

}
