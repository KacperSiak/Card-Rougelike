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
        RoundManager.OnCombatEnd += NoDiscardFightWon;
        RoundManager.OnCombatEnd += QuestReset;
        DiscardManager.OnDiscard.AddListener(QuestFailed);
    }

    public override void Unregister(ChallengeContext ctx)
    {
        RoundManager.OnCombatEnd -= NoDiscardFightWon;
        RoundManager.OnCombatEnd -= QuestReset;
        DiscardManager.OnDiscard.RemoveListener(QuestFailed);
    }

    private void NoDiscardFightWon()
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
