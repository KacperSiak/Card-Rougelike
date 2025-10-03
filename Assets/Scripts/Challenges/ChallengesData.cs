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
    [field: SerializeField] public string challengeName { get; private set; }
    [field: SerializeField, TextArea] public string challengeDescription { get; private set; }
    [field: SerializeField] public Difficulty difficulty { get; private set; }
    [field: SerializeField] public Sprite challIcon { get; private set; }
    [field: SerializeField] public int col { get; private set; }
    [field: SerializeField] public int row { get; private set; }
    [SerializeReference] public List<Challenges> challenge;
}

public class ChallengeContext
{
    public PlayerManager player;
    public MapManager mapManager;
    public ChallengesMenager challengesMenager;
    public DamageManager damage;

    public ChallengeContext()
    {
        player = PlayerManager.Instance;
        mapManager = MapManager.Instance;
        challengesMenager = ChallengesMenager.Instance;
        damage = DamageManager.Instance;    
    }
}

[Serializable]
public abstract class Challenges
{
    public abstract void Register(ChallengeContext ctx);
    public abstract void Unregister(ChallengeContext ctx);
}
