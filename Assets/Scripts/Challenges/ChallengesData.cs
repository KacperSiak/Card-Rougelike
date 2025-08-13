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
    public Sprite challIcon;
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
    private bool _questInfo;
    private ChallengeContext context;
    public override void Register(ChallengeContext ctx)
    {
        context = ctx;
        _questInfo = false;
        RoundManager.OnCombatEnd += NoConsumableFightWon;
        RoundManager.OnCombatEnd += QuestReset;
        // discard used subscribe for questundone;
    }

    public override void Unregister(ChallengeContext ctx)
    {
        RoundManager.OnCombatEnd -= NoConsumableFightWon;
        RoundManager.OnCombatEnd -= QuestReset;
        // discard used remove subscribe for questundone;
    }

    private void NoConsumableFightWon()
    {
        if (_questInfo)
        {
            context.challengesMenager.AddReward();
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

    private void QuestUndone()
    {
        _questInfo = false;
    }

    private void QuestReset()
    {
        _questInfo = true;
    }
}

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

[Serializable]
public class NoJoke : Challenges
{
    private bool _questInfo;
    private ChallengeContext context;
    public override void Register(ChallengeContext ctx)
    {
        context = ctx;
        _questInfo = false; 
        RoundManager.OnCombatEnd += NoJokerFightWon;
        RoundManager.OnCombatEnd += QuestReset;
        // joker used subscribe for questundone;
    }

    public override void Unregister(ChallengeContext ctx)
    {
        RoundManager.OnCombatEnd -= NoJokerFightWon;
        RoundManager.OnCombatEnd -= QuestReset;
        // joker used remove subscribe for questundone;
    }

    private void NoJokerFightWon()
    {
        if (_questInfo)
        {
            context.challengesMenager.AddReward();
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

[Serializable]
public class OnlyRedCards : Challenges
{
    private bool _questInfo;
    private ChallengeContext context;
    public override void Register(ChallengeContext ctx)
    {
        context = ctx;
        _questInfo = false;
        RoundManager.OnCombatEnd += NoJokerFightWon;
        RoundManager.OnCombatEnd += QuestReset;
        // Black card used subscribe for questundone;
    }

    public override void Unregister(ChallengeContext ctx)
    {
        RoundManager.OnCombatEnd -= NoJokerFightWon;
        RoundManager.OnCombatEnd -= QuestReset;
        // Black card used remove subscribe for questundone;
    }

    private void NoJokerFightWon()
    {
        if (_questInfo)
        {
            context.challengesMenager.AddReward();
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

[Serializable]
public class OnlyBlackCards : Challenges
{
    private bool _questInfo;
    private ChallengeContext context;
    public override void Register(ChallengeContext ctx)
    {
        context = ctx;
        _questInfo = false;
        RoundManager.OnCombatEnd += NoJokerFightWon;
        RoundManager.OnCombatEnd += QuestReset;
        // Red card used subscribe for questundone;
    }

    public override void Unregister(ChallengeContext ctx)
    {
        RoundManager.OnCombatEnd -= NoJokerFightWon;
        RoundManager.OnCombatEnd -= QuestReset;
        // Red card used remove subscribe for questundone;
    }

    private void NoJokerFightWon()
    {
        if (_questInfo)
        {
            context.challengesMenager.AddReward();
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

[Serializable]
public class NoAssetCards : Challenges
{
    private bool _questInfo;
    private ChallengeContext context;
    public override void Register(ChallengeContext ctx)
    {
        context = ctx;
        _questInfo = false;
        RoundManager.OnCombatEnd += NoJokerFightWon;
        RoundManager.OnCombatEnd += QuestReset;
        // Asset card used subscribe for questundone;
    }

    public override void Unregister(ChallengeContext ctx)
    {
        RoundManager.OnCombatEnd -= NoJokerFightWon;
        RoundManager.OnCombatEnd -= QuestReset;
        // Asset card used remove subscribe for questundone;
    }

    private void NoJokerFightWon()
    {
        if (_questInfo)
        {
            context.challengesMenager.AddReward();
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