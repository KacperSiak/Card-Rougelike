using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PotionData", menuName = "Shop/PotionData")]
public class PotionData : ShopItem
{
    [field: SerializeField] public string potionName;
    [field: SerializeField, TextArea] public string potionDescription;
    [field: SerializeField] public int buyPrice;
    [field: SerializeField] public int fightsLeft;
    [field: SerializeField] public Sprite potionIcon;
    [SerializeReference] public List<PotionEffects> potionEffects;
}

public class PotionContext
{
    public PlayerManager player;
    public ShopMenager shopManager;
    public GamblingManager gambler;
    public BuffsAndDebuffsManager buffsAndDebuffsManager;

    public PotionContext()
    {
        player = PlayerManager.Instance;
        shopManager = ShopMenager.Instance;
        gambler = GamblingManager.Instance;
        buffsAndDebuffsManager = BuffsAndDebuffsManager.Instance;
    }
}

[Serializable]
public abstract class PotionEffects
{
    public abstract void Register(PotionContext ctx, int fightLeft);
    public abstract void Unregister(PotionContext ctx);
    public abstract void FightsLeft();
}
