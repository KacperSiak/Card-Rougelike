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
        RoundManager.OnCombatEnd += NoTrumpCardFightWon;
        RoundManager.OnCombatEnd += QuestReset;
        CardHolderLogic.OnTrumpSuitPlayed.AddListener(QuestFailed);
    }

    public override void Unregister(ChallengeContext ctx)
    {
        RoundManager.OnCombatEnd -= NoTrumpCardFightWon;
        RoundManager.OnCombatEnd -= QuestReset;
        CardHolderLogic.OnTrumpSuitPlayed.RemoveListener(QuestFailed);
    }

    private void NoTrumpCardFightWon()
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