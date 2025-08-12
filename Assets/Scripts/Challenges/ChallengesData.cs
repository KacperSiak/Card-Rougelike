using System;
using System.Collections.Generic;
using UnityEngine;

public enum Difficulty
{
    easy,
    normal,
    hard
}

[CreateAssetMenu(fileName = "ChallengesData", menuName = "Challenge/ChallengesData")]
public class ChallengesData : ScriptableObject
{
    public string challengeName;
    [TextArea] public string challengeDescription;
    public Difficulty difficulty;
    [TextArea] public string rewardDestcription;
    [SerializeReference] public List<Challenges> challenge;
}

public class ChallengeContext
{
    public PlayerOverlay player;
    public MapManager mapManager;
    public ChallengesMenager challengesMenager;

    public ChallengeContext()
    {
        player = PlayerOverlay.instance;
        mapManager = MapManager.instance;
        challengesMenager = ChallengesMenager.instance;
    }
}

[Serializable]
public abstract class Challenges
{
    public abstract void Register(ChallengeContext ctx);
    public abstract void Unregister(ChallengeContext ctx);
}

[Serializable]
public class NoDiscard : Challenges
{
    public override void Register(ChallengeContext ctx)
    {
        // listen to NoDicardFightEvent
        // OnNoDiscardFight.AddListener(NoDiscardFightWon)
    }

    public override void Unregister(ChallengeContext ctx)
    {
        throw new NotImplementedException();
    }

    private void NoDisardFightWon(ChallengeContext ctx)
    {
        ctx.challengesMenager.AddReward();
    }
}