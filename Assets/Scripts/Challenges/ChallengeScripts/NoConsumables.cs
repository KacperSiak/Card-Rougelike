using System;

[Serializable]
public class NoConsumables : Challenges
{
    private bool _questInfo;
    private ChallengeContext context;
    public override void Register(ChallengeContext ctx)
    {
        context = ctx;
        _questInfo = true;
        RoundManager.OnCombatEnd += NoConsumableFightWon;
        RoundManager.OnCombatEnd += QuestReset;
        ConsCardProperties.OnConsumableUse.AddListener(QuestFailed);
    }

    public override void Unregister(ChallengeContext ctx)
    {
        RoundManager.OnCombatEnd -= NoConsumableFightWon;
        RoundManager.OnCombatEnd -= QuestReset;
        ConsCardProperties.OnConsumableUse.RemoveListener(QuestFailed);
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
