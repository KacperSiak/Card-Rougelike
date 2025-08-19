using System;

[Serializable]
public class OnlyBlackCards : Challenges
{
    private bool _questInfo;
    private ChallengeContext context;
    public override void Register(ChallengeContext ctx)
    {
        context = ctx;
        _questInfo = false;
        RoundManager.OnCombatEnd += NoJokerFightWon;
        RoundManager.OnCombatEnd += QuestReset;
        // Red card used subscribe for questundone;
    }

    public override void Unregister(ChallengeContext ctx)
    {
        RoundManager.OnCombatEnd -= NoJokerFightWon;
        RoundManager.OnCombatEnd -= QuestReset;
        // Red card used remove subscribe for questundone;
    }

    private void NoJokerFightWon()
    {
        if (_questInfo)
        {
            context.challengesMenager.AddReward();
            Unregister(context);
        }
    }

    private void QuestUndone()
    {
        _questInfo = false;
    }

    private void QuestReset()
    {
        _questInfo = true;
    }
}
