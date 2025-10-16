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
        RoundManager.OnCombatEnd += OnlyBlackPlayed;
        RoundManager.OnCombatEnd += QuestReset;
        CardHolderLogic.OnRedPlay.AddListener(QuestUndone);
    }

    public override void Unregister(ChallengeContext ctx)
    {
        RoundManager.OnCombatEnd -= OnlyBlackPlayed;
        RoundManager.OnCombatEnd -= QuestReset;
        CardHolderLogic.OnRedPlay.RemoveListener(QuestUndone);
    }

    private void OnlyBlackPlayed()
    {
        if (_questInfo)
        {
            context.challengesMenager.AddRewardForChall();
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
