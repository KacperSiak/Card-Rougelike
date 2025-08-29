using System;

[Serializable]
public class NoDiscard : Challenges
{
    private bool _questInfo;
    private ChallengeContext context;
    public override void Register(ChallengeContext ctx)
    {
        context = ctx;
        _questInfo = false;
        RoundManager.OnCombatEnd += NoConsumableFightWon;
        RoundManager.OnCombatEnd += QuestReset;
        // discard used subscribe for questundone;
    }

    public override void Unregister(ChallengeContext ctx)
    {
        RoundManager.OnCombatEnd -= NoConsumableFightWon;
        RoundManager.OnCombatEnd -= QuestReset;
        // discard used remove subscribe for questundone;
    }

    private void NoConsumableFightWon()
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
