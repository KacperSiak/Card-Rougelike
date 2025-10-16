using System;
using System.Collections.Generic;
using UnityEngine.Events;
using Ink.Parsed;

[Serializable]
public class TwoForOne : CardEffect
{
    private ConsumableContext _context;
    private List<RelicData> _relics;
    private Rarity _neededRarity;

    public override void ApplyEffect(ConsumableContext ctx)
    {
        if(CheckRarity())
        {
            _relics = DeleteRelics();

            foreach (var relic in _relics)
            {
                RelicEventsSystem.RelicRemoved(relic);
            }

            _neededRarity = ctx.consAndRel.RelicRarityNeed(_relics[0].rarity);
            RelicEventsSystem.RelicAcquired(ctx.consAndRel.RollRarityRelic(_neededRarity));
        }
    }

    private bool CheckRarity()
    {
        for (int i = 0; i < _context.consAndRel.currentRelics.Count; i++)
        {
            for (int j = 0; j < _context.consAndRel.currentRelics.Count; j++)
            {
                if (_context.consAndRel.currentRelics[i].rarity == _context.consAndRel.currentRelics[j].rarity && _context.consAndRel.currentRelics[i].rarity != Rarity.Legendary && i != j)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private List<RelicData> DeleteRelics()
    {
        List<RelicData> temporaryRelics = new List<RelicData>();
        bool found = false;

        for (int i = 0; i < _context.consAndRel.currentRelics.Count && !found; i++)
        {
            for (int j = 0; j < _context.consAndRel.currentRelics.Count; j++)
            {
                if (_context.consAndRel.currentRelics[i].rarity == _context.consAndRel.currentRelics[j].rarity && _context.consAndRel.currentRelics[i].rarity != Rarity.Legendary && i != j)
                {
                    temporaryRelics.Add(_context.consAndRel.currentRelics[i]);
                    temporaryRelics.Add(_context.consAndRel.currentRelics[j]);
                    found = true;
                    break;
                }
            }
        }

        return temporaryRelics;
    }
}
