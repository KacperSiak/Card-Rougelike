using System;

[Serializable]
public class HalfEnemyHP : Challenges
{
    private ChallengeContext context;
    public override void Register(ChallengeContext ctx)
    {
        context = ctx;
        // OnHPFallTo50 += ChallengeComplete;
    }
    public override void Unregister(ChallengeContext ctx)
    {
        // OnHPFallTo50 -= ChallengeCOmplete;
    }

    private void ChallengeComplete()
    {
        context.challengesMenager.AddReward();
        Unregister(context);
    }
}
