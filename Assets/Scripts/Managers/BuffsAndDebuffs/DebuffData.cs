using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DebuffData", menuName = "Debuff/DebuffData")]
public class DebuffData : ScriptableObject
{
    public string debuffName;
    [TextArea] public string debuffDesc;
    public Sprite debuffIcon;
    public int debuffTurns;
    [SerializeReference] public List<DebuffEffects> debuffs;
}

public class DebuffContext
{
    public PlayerOverlay player;
    public ShopMenager shopManager;

    public DebuffContext()
    {
        player = PlayerOverlay.instance;
        shopManager = ShopMenager.instance;
    }
}

[Serializable]
public abstract class DebuffEffects
{
    public abstract void Register(DebuffContext ctx, int turnCounter);
    public abstract void Unregister(DebuffContext ctx);
    public abstract void TurnsLeft();
}