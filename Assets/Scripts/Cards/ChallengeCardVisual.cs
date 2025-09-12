using Ink.Parsed;
using UnityEngine;

public class ChallengeCardVisual : MonoBehaviour
{
    [SerializeField] private MeshRenderer front;
    [SerializeField] private MeshRenderer back;

    [SerializeField] private Texture2D easy;
    [SerializeField] private Texture2D normal;
    [SerializeField] private Texture2D hard;

    public void SetupChalss(int col, int row)
    {
        MaterialSetupManager.Instance.SetupVisual(front, col, row);
    }

    public void SetupBack(int number)
    {
        Texture2D chosen = number switch
        {
            0 => easy,
            1 => normal,
            2 => hard,
            _ => easy
        };
    }
}
