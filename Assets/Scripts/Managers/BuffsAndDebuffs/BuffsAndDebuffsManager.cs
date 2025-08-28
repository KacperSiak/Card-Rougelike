using System.Collections.Generic;
using UnityEngine;

public class BuffsAndDebuffsManager : PersistenSingleton<BuffsAndDebuffsManager>
{
    [SerializeField] private List<DebuffData> _debuffList;
    [SerializeField] private BuffAndDebuffUI _buffAndDebuffUI;

    public List<DebuffData> _currentDebuffs;
    public List<PotionData> _currentPotionEffects;

    private bool _debuffProetection = false;

    protected override void OnSingletonAwake() { }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    public void ApplyBuff(PotionData potion)
    {
        var ctx = new PotionContext();
        foreach (var effect in potion.potionEffects)
        {
            effect.Register(ctx, potion.fightsLeft);
        }
        _buffAndDebuffUI.AddBuffIcon(potion);

        _currentPotionEffects.Add(potion);
    }

    public void ApplyDebuff(DebuffData debuff)
    {
        var ctx = new DebuffContext();
        foreach (var effect in debuff.debuffs)
        {
            effect.Register(ctx, debuff.debuffTurns);
        }
        _buffAndDebuffUI.AddDebuffIcon(debuff);
        _currentDebuffs.Add(debuff);
    }

    public void RollDebuff()
    {
        if (!_debuffProetection)
        {
            int index = Random.Range(0, _debuffList.Count);
            ApplyDebuff(_debuffList[index]);
        }
        else
        {
            Debug.Log("Protection is Enabled");
        }
    }

    public void RemoveRandomDebuff()
    {
        int index = Random.Range(0, _currentDebuffs.Count);
        _currentDebuffs.RemoveAt(index);
    }

    public void DebuffProtectionEnable()
    {
        _debuffProetection = true;
    }

    public void DebuffProtectionDisable()
    {
        _debuffProetection = false;
    }


}
