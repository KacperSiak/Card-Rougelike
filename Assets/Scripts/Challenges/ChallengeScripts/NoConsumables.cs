using System;

[Serializable]
public class NoConsumables : Challenges
{
    private bool _questInfo;
    private ChallengeContext context;
    public override void Register(ChallengeContext ctx)
    {
        context = ctx;
        _questInfo = false;
        RoundManager.OnCombatEnd += NoConsumableFightWon;
        RoundManager.OnCombatEnd += QuestReset;
        // consumable used subscribe for questundone;
    }

    public override void Unregister(ChallengeContext ctx)
    {
        RoundManager.OnCombatEnd -= NoConsumableFightWon;
        RoundManager.OnCombatEnd -= QuestReset;
        // consumable used remove subscribe for questundone;
    }

    private void NoConsumableFightWon()
    {
        if (_questInfo)
        {
            context.challengesMenager.AddReward();
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
