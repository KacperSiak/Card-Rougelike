using System.Collections.Generic;
using UnityEngine;

public class BuffsAndDebuffsManager : MonoBehaviour
{
    [SerializeField] private List<DebuffData> _debuffList;
    [SerializeField] private BuffAndDebuffUI _buffAndDebuffUI;
    public static BuffsAndDebuffsManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("I found another Buff and Debuff Menager");
        }
    }

    public void ApplyBuff(PotionData potion)
    {
        var ctx = new PotionContext
        {
            player = PlayerOverlay.instance
        };
        foreach (var effect in potion.potionEffects)
        {
            effect.Register(ctx, potion.fightsLeft);
        }
        _buffAndDebuffUI.AddBuffIcon(potion);

    }

    public void ApplyDebuff(DebuffData debuff)
    {
        var ctx = new DebuffContext
        {
            player = PlayerOverlay.instance
        };
        foreach (var effect in debuff.debuffs)
        {
            effect.Register(ctx, debuff.debuffTurns);
        }
    }

    public void RollDebuff()
    {
        int index = Random.Range(0, _debuffList.Count);
        ApplyDebuff(_debuffList[index]);
    }
}
