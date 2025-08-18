using System;

[Serializable]
public class ThreeSinglesInARow : Challenges
{
    private int _soloCards;
    private ChallengeContext context;
    public override void Register(ChallengeContext ctx)
    {
        context = ctx;
        _soloCards = 0;
        // onsolocard event += Quest Counter
        // onOtherCardd event += Quest Reset
    }

    public override void Unregister(ChallengeContext ctx)
    {
        // onsolocard event -= Quest Counter
        // onOtherCardd event -= Quest Reset
    }

    private void QuestCounter()
    {
        _soloCards++;
        if (_soloCards >= 3)
        {
            context.challengesMenager.AddReward();
            Unregister(context);
        }
    }

    private void QuestReset()
    {
        _soloCards = 0;
    }
}
