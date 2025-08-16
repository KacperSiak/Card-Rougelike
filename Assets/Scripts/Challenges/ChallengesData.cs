using System;
using System.Collections.Generic;
using UnityEngine;

public enum Difficulty
{
    Easy,
    Normal,
    Hard
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
