using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PotionData", menuName = "Shop/PotionData")]
public class PotionData : ShopItem
{
    public string potionName;
    [TextArea] public string potionDescription;
    public int buyPrice;
    public int fightsLeft;
    public Sprite potionIcon;
    [SerializeReference] public List<PotionEffects> potionEffects;
}

public class PotionContext
{
    public PlayerOverlay player;
    public ShopMenager shopManager;
    public GamblingManager gambler;
    public BuffsAndDebuffsManager buffsAndDebuffsManager;

    public PotionContext()
    {
        player = PlayerOverlay.instance;
        shopManager = ShopMenager.instance;
        gambler = GamblingManager.instance;
        buffsAndDebuffsManager = BuffsAndDebuffsManager.instance;
    }
}

[Serializable]
public abstract class PotionEffects
{
    public abstract void Register(PotionContext ctx, int fightLeft);
    public abstract void Unregister(PotionContext ctx);
    public abstract void FightsLeft();
}
