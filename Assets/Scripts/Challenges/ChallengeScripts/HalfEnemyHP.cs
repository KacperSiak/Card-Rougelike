using System;

[Serializable]
public class HalfEnemyHP : Challenges
{
    private ChallengeContext context;
    public override void Register(ChallengeContext ctx)
    {
        context = ctx;
        DamageManager.OnEnemyHalfHP.AddListener(ChallengeComplete);
    }
    public override void Unregister(ChallengeContext ctx)
    {
        DamageManager.OnEnemyHalfHP.RemoveAllListeners();
    }

    private void ChallengeComplete()
    {
        context.challengesMenager.AddRewardForChall();
        Unregister(context);
    }
}
