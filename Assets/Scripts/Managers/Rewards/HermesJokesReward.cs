using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class HermesJokesReward : Rewards
{
    [SerializeField] private List<HermesJoke> hermesJokes;
    public override void AddReward(RewardContext ctx)
    {
        HermesJoke joke = hermesJokes[UnityEngine.Random.Range(0, hermesJokes.Count)];
        ctx.gambling.ShowHermesJoke(joke);
    }
}
