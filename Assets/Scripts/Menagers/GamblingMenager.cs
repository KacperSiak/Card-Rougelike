using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class GamblingMenager : MonoBehaviour
{
    [SerializeField] private Button _roll;
    private List<Action> _rollOutcomes;

    private void Start()
    {
        _rollOutcomes = new List<Action>
        {
            Option1,
            Option2,
            Option3,
            Option4,
            Option5,
        };

        _roll.onClick.AddListener(RollForPrize);
    }

    private void RollForPrize()
    {
        int index = UnityEngine.Random.Range(0, _rollOutcomes.Count);
        _rollOutcomes[index].Invoke();
    }

    private void Option1()
    {
        Debug.Log("Option 1 rolled");
    }

    private void Option2()
    {
        Debug.Log("Option 2 rolled");
    }

    private void Option3()
    {
        Debug.Log("Option 3 rolled");
    }

    private void Option4()
    {
        Debug.Log("Option 4 rolled");
    }

    private void Option5()
    {
        Debug.Log("Option 5 rolled");
    }

}
