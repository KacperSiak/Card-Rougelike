using System;
using UnityEngine;

public class TrumpSuitSetup : MonoBehaviour
{
    [SerializeField] private MeshRenderer _card;
    [SerializeField] private Animator _animator;

    public Suits ChooseSuit()
    {
        var suit = (Suits)UnityEngine.Random.Range(0, Enum.GetValues(typeof(Suits)).Length - 1);
        return suit;
    }

    public void SetUpTrumpCard(int suitKey)
    {
        MaterialSetupManager.Instance.SetupVisual(_card, col: suitKey, row: 0);
        _animator.SetTrigger("reveal");
    }
}
