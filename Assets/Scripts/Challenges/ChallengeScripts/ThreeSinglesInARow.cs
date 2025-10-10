using System;
using UnityEngine;

[Serializable]
public class ThreeSinglesInARow : Challenges
{
    public int _maxSoloCards;
    private int _soloCards;
    private ChallengeContext context;
    public override void Register(ChallengeContext ctx)
    {
        context = ctx;
        _soloCards = 0;
        context.challengesMenager.ChallengeProgressUpdate(_soloCards, _maxSoloCards);
        DamageManager.OnSingleDestroy.AddListener(QuestCounter);
        DamageManager.OnSequenceDestroy.AddListener(QuestReset);
    }

    public override void Unregister(ChallengeContext ctx)
    {
        DamageManager.OnSingleDestroy.RemoveListener(QuestCounter);
        DamageManager.OnSequenceDestroy.RemoveListener(QuestReset);
    }

    private void QuestCounter()
    {
        _soloCards++;
        context.challengesMenager.ChallengeProgressUpdate(_soloCards, _maxSoloCards);
        if (_soloCards >= 3)
        {
            context.challengesMenager.AddRewardForChall();
            Unregister(context);
        }
    }

    private void QuestReset()
    {
        _soloCards = 0;
        context.challengesMenager.ChallengeProgressUpdate(_soloCards, _maxSoloCards);
    }
}
