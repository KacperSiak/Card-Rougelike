using System;

[Serializable]
public class OnlyRedCards : Challenges
{
    private bool _questInfo;
    private ChallengeContext context;
    public override void Register(ChallengeContext ctx)
    {
        context = ctx;
        _questInfo = false;
        RoundManager.OnCombatEnd += OnlyRedPlayed;
        RoundManager.OnCombatEnd += QuestReset;
        CardHolderLogic.OnBlackPlay.AddListener(QuestUndone);
    }

    public override void Unregister(ChallengeContext ctx)
    {
        RoundManager.OnCombatEnd -= OnlyRedPlayed;
        RoundManager.OnCombatEnd -= QuestReset;
        CardHolderLogic.OnBlackPlay.RemoveListener(QuestUndone);
    }

    private void OnlyRedPlayed()
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
