using System;

[Serializable]
public class NoAssetCards : Challenges
{
    private bool _questInfo;
    private ChallengeContext context;
    public override void Register(ChallengeContext ctx)
    {
        context = ctx;
        _questInfo = false;
        RoundManager.OnCombatEnd += NoJokerFightWon;
        RoundManager.OnCombatEnd += QuestReset;
        // Asset card used subscribe for questundone;
    }

    public override void Unregister(ChallengeContext ctx)
    {
        RoundManager.OnCombatEnd -= NoJokerFightWon;
        RoundManager.OnCombatEnd -= QuestReset;
        // Asset card used remove subscribe for questundone;
    }

    private void NoJokerFightWon()
    {
        if (_questInfo)
        {
            context.challengesMenager.AddRewardForChall();
            Unregister(context);
        }
    }

    private void QuestFailed()
    {
        _questInfo = false;
    }

    private void QuestReset()
    {
        _questInfo = true;
    }
}